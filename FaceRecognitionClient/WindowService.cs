using FaceRecognitionClient;
using FaceRecognitionClient.InternalDataModels;
using FaceRecognitionClient.MVVMStructures.ViewModels.Attendance;
using FaceRecognitionClient.MVVMStructures.ViewModels.Authentication;
using FaceRecognitionClient.MVVMStructures.ViewModels.FaceRecognition;
using FaceRecognitionClient.MVVMStructures.ViewModels.Gallery;
using FaceRecognitionClient.MVVMStructures.ViewModels.NavigationWindow;
using FaceRecognitionClient.MVVMStructures.ViewModels.PersonProfile;
using FaceRecognitionClient.MVVMStructures.ViewModels.Disconnected;
using FaceRecognitionClient.MVVMStructures.Views.Attendance;
using FaceRecognitionClient.MVVMStructures.Views.PersonProfile;
using FaceRecognitionClient.Services.GalleryService;
using FaceRecognitionClient.StateMachine;
using FaceRecognitionClient.Views.Authentication;
using FaceRecognitionClient.Views;
using System.Windows;
using FaceRecognitionClient.ClientLogger;

namespace LogInClient
{
    internal class WindowService
    {
        // Core services and shared state
        private IGalleryService m_GalleryService;
        private Mapper m_Mapper;
        private INetworkFacade m_NetworkFacade;
        private SharedImageStore m_SharedImageStore;
        private UserSession m_UserSession;

        // ViewModels
        private LogInViewModel m_LogInViewModel;
        private SignUpViewModel m_SignUpViewModel;
        private FaceRecognitionViewModel m_FaceRecognitionViewModel;
        private CameraCaptureViewModel m_CameraCaptureViewModel;
        private CaptchaViewModel m_CaptchaViewModel;
        private EmailVerificationViewModel m_EmailVerificationViewModel;
        private GalleryViewModel m_GalleryViewModel;
        private ForgotPasswordViewModel m_ForgotPasswordViewModel;
        private GeneralAttendanceViewModel m_GeneralAttendanceViewModel;
        private NavigationWindowViewModel m_NavigationWindowViewModel;
        private DisconnectedViewModel m_DisconnectedViewModel;

        // Views (Windows)
        private LogInWindow m_LogInWindow;
        private SignUpWindow m_SignUpWindow;
        private FaceRecoginitionWindow m_FaceRecognitionWindow;
        private CameraCaptureWindow m_CameraCaptureWindow;
        private CaptchaWindow m_CaptchaWindow;
        private EmailVerificationWindow m_EmailVerificationWindow;
        private GalleryWindow m_GalleryWindow;
        private ForgotPasswordWindow m_ForgotPasswordWindow;
        private GeneralAttendanceView m_GeneralAttendanceWindow;
        private NavigationWindow m_NavigationWindow;
        private DisconnectedWindow m_DisconnectedWindow;

        // Navigation state system
        private IStateMachine<ApplicationState, ApplicationTrigger> m_WindowNavigationSystem;
        private List<IStateNotifier> m_StateNotifiers;
        private List<IDetailNotifier<AdvancedPersonDataWithImage>> m_DetailNotifiers;

        // Used to store a record selected for detail view (either from face recognition or gallery)
        private AdvancedPersonDataWithImage m_PendingDetailsRecord;
        private GalleryImage m_PendingGalleryImage;

        /// <summary>
        /// Constructor initializes all services, windows, view models, and launches the LogIn window.
        /// </summary>
        public WindowService()
        {
            m_WindowNavigationSystem = BuildStateMachine();

            InitializeKeyComponents();

            m_DisconnectedViewModel = new DisconnectedViewModel();
            m_DisconnectedWindow = new DisconnectedWindow { DataContext = m_DisconnectedViewModel };

            SubscribeToAllCriticalEvents();
            m_LogInWindow.Show();
        }

        private void SubscribeToAllCriticalEvents()
        {
            m_StateNotifiers.AddRange(new IStateNotifier[]
            {
                m_LogInViewModel,
                m_SignUpViewModel,
                m_FaceRecognitionViewModel,
                m_CameraCaptureViewModel,
                m_CaptchaViewModel,
                m_EmailVerificationViewModel,
                m_GalleryViewModel,
                m_ForgotPasswordViewModel,
                m_NavigationWindowViewModel,
                m_DisconnectedViewModel,
                m_GeneralAttendanceViewModel
            });

            m_DetailNotifiers.AddRange(new IDetailNotifier<AdvancedPersonDataWithImage>[]
            {
                m_FaceRecognitionViewModel,
                m_GalleryViewModel,
                m_GeneralAttendanceViewModel
            });

            SubscribeToStateNotifiers();
            SubscribeToDetailNotifiers();
            SubscribeToNetworkEvents();
        }

        /// <summary>
        /// Subscribes all view models that can raise navigation triggers to the state machine.
        /// </summary>
        private void SubscribeToStateNotifiers()
        {
            foreach (var notifier in m_StateNotifiers)
            {
                notifier.OnTriggerOccurred += HandleTrigger;
            }
        }

        private void SubscribeToDetailNotifiers()
        {
            foreach (var notifier in m_DetailNotifiers)
            {
                notifier.OnDetailRequested += HandleDetailRequest;
            }
        }

        private void SubscribeToNetworkEvents()
        {
            if (m_NetworkFacade is NetworkFacade facade)
            {
                facade.OnServerDisconnected += HandleServerDisconnect;
            }
        }

        /// <summary>
        /// Configures a finite state machine to define how windows transition between each other.
        /// </summary>
        private IStateMachine<ApplicationState, ApplicationTrigger> BuildStateMachine()
        {
            var stateMachine = new StateMachine<ApplicationState, ApplicationTrigger>(ApplicationState.LogInWindow);

            // Transitions between windows
            stateMachine.AddTransition(ApplicationState.LogInWindow, ApplicationTrigger.SignUpRequested, ApplicationState.SignUpWindow);
            stateMachine.AddTransition(ApplicationState.SignUpWindow, ApplicationTrigger.LogInRequested, ApplicationState.LogInWindow);
            stateMachine.AddTransition(ApplicationState.LogInWindow, ApplicationTrigger.LoginSuccessful, ApplicationState.CaptchaWindow);
            stateMachine.AddTransition(ApplicationState.SignUpWindow, ApplicationTrigger.SignUpSuccessful, ApplicationState.CaptchaWindow);
            stateMachine.AddTransition(ApplicationState.CaptchaWindow, ApplicationTrigger.CaptchaPassed, ApplicationState.EmailVerificationWindow);
            stateMachine.AddTransition(ApplicationState.EmailVerificationWindow, ApplicationTrigger.EmailVerified, ApplicationState.NavigationWindow);
            stateMachine.AddTransition(ApplicationState.LogInWindow, ApplicationTrigger.DeveloperEntered, ApplicationState.NavigationWindow);
            stateMachine.AddTransition(ApplicationState.FaceRecognitionWindow, ApplicationTrigger.CameraCaptureRequested, ApplicationState.CameraCaptureWindow);
            stateMachine.AddTransition(ApplicationState.FaceRecognitionWindow, ApplicationTrigger.GalleryRequested, ApplicationState.GalleryWindow);
            stateMachine.AddTransition(ApplicationState.FaceRecognitionWindow, ApplicationTrigger.AttendanceRequested, ApplicationState.AttendanceWindow);
            stateMachine.AddTransition(ApplicationState.FaceRecognitionWindow, ApplicationTrigger.NavigationRequested, ApplicationState.NavigationWindow);
            stateMachine.AddTransition(ApplicationState.NavigationWindow, ApplicationTrigger.FaceRecognitionRequested, ApplicationState.FaceRecognitionWindow);
            stateMachine.AddTransition(ApplicationState.NavigationWindow, ApplicationTrigger.GalleryRequested, ApplicationState.GalleryWindow);
            stateMachine.AddTransition(ApplicationState.NavigationWindow, ApplicationTrigger.AttendanceRequested, ApplicationState.AttendanceWindow);
            stateMachine.AddTransition(ApplicationState.GalleryWindow, ApplicationTrigger.FaceRecognitionRequested, ApplicationState.FaceRecognitionWindow);
            stateMachine.AddTransition(ApplicationState.GalleryWindow, ApplicationTrigger.NavigationRequested, ApplicationState.NavigationWindow);
            stateMachine.AddTransition(ApplicationState.CameraCaptureWindow, ApplicationTrigger.FaceRecognitionRequested, ApplicationState.FaceRecognitionWindow);
            stateMachine.AddTransition(ApplicationState.AttendanceWindow, ApplicationTrigger.NavigationRequested, ApplicationState.NavigationWindow);
            stateMachine.AddTransition(ApplicationState.LogInWindow, ApplicationTrigger.LogInFailed, ApplicationState.LogInWindow);
            stateMachine.AddTransition(ApplicationState.ForgotPasswordWindow, ApplicationTrigger.LogInRequested, ApplicationState.LogInWindow);
            stateMachine.AddTransition(ApplicationState.LogInWindow, ApplicationTrigger.ForgotPasswordRequested, ApplicationState.ForgotPasswordWindow);
            stateMachine.AddTransition(ApplicationState.ForgotPasswordWindow, ApplicationTrigger.PasswordResetSuccessful, ApplicationState.LogInWindow);
            stateMachine.AddTransition(ApplicationState.CameraCaptureWindow, ApplicationTrigger.PhotoAccepted, ApplicationState.FaceRecognitionWindow, () =>
            {
                UpdateUI(m_FaceRecognitionViewModel.LoadCapturedImage);
                UpdateUI(m_CameraCaptureViewModel.StopCamera);
            });

            foreach (ApplicationState state in Enum.GetValues<ApplicationState>())
            {
                if (state != ApplicationState.DisconnectedWindow)
                {
                    stateMachine.AddTransition(state, ApplicationTrigger.UserDisconnected, ApplicationState.DisconnectedWindow);
                }
            }

            stateMachine.AddTransition(ApplicationState.NavigationWindow, ApplicationTrigger.LogOutRequested, ApplicationState.LogInWindow, InitializeViewsAndViewModels);

            stateMachine.AddTransition(ApplicationState.DisconnectedWindow, ApplicationTrigger.LogInRequested, ApplicationState.LogInWindow);

            // Internal transitions for face detail viewing
            stateMachine.AddInternalTransition(ApplicationState.FaceRecognitionWindow, ApplicationTrigger.ShowPersonDetails, () => ShowPersonProfileWindow(m_PendingDetailsRecord));
            stateMachine.AddInternalTransition(ApplicationState.GalleryWindow, ApplicationTrigger.ShowPersonDetails, () => ShowPersonProfileWindow(m_PendingDetailsRecord));
            stateMachine.AddInternalTransition(ApplicationState.AttendanceWindow, ApplicationTrigger.ShowPersonDetails, () => ShowPersonProfileWindow(m_PendingDetailsRecord));
            // Entry actions
            stateMachine.AddStateEntryAction(ApplicationState.LogInWindow, () => UpdateUI(() => m_LogInWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.SignUpWindow, () => UpdateUI(() => m_SignUpWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.CaptchaWindow, () => UpdateUI(() => m_CaptchaWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.EmailVerificationWindow, () => UpdateUI(() => InitializeEmailVerification()));
            stateMachine.AddStateEntryAction(ApplicationState.FaceRecognitionWindow, () => UpdateUI(() => m_FaceRecognitionWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.CameraCaptureWindow, () => UpdateUI(() => m_CameraCaptureWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.GalleryWindow, () => UpdateUI(() => InitializeGallery()));
            stateMachine.AddStateEntryAction(ApplicationState.ForgotPasswordWindow, () => UpdateUI(() => m_ForgotPasswordWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.AttendanceWindow, () => UpdateUI(() => m_GeneralAttendanceWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.NavigationWindow, () => UpdateUI(() => m_NavigationWindow.Show()));
            stateMachine.AddStateEntryAction(ApplicationState.DisconnectedWindow, () => UpdateUI(() => ReConnect()));

            // Exit actions
            stateMachine.AddStateExitAction(ApplicationState.LogInWindow, () => UpdateUI(() => m_LogInWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.SignUpWindow, () => UpdateUI(() => m_SignUpWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.CaptchaWindow, () => UpdateUI(() => m_CaptchaWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.EmailVerificationWindow, () => UpdateUI(() => m_EmailVerificationWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.FaceRecognitionWindow, () => UpdateUI(() => m_FaceRecognitionWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.CameraCaptureWindow, () => UpdateUI(() => m_CameraCaptureWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.GalleryWindow, () => UpdateUI(() => m_GalleryWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.ForgotPasswordWindow, () => UpdateUI(() => m_ForgotPasswordWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.AttendanceWindow, () => UpdateUI(() => m_GeneralAttendanceWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.NavigationWindow, () => UpdateUI(() => m_NavigationWindow.Hide()));
            stateMachine.AddStateExitAction(ApplicationState.DisconnectedWindow, () => UpdateUI(() => m_DisconnectedWindow.Hide()));

            return stateMachine;
        }

        /// <summary>
        /// Ensures UI actions run on the main UI thread.
        /// </summary>
        private void UpdateUI(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                action();
            else
                Application.Current.Dispatcher.Invoke(action);
        }

        /// <summary>
        /// Opens the face details window for the selected person.
        /// </summary>
        private void ShowPersonProfileWindow(AdvancedPersonDataWithImage record)
        {
            var vm = new PersonProfileViewModel(m_NetworkFacade, m_Mapper, record);
            var window = new PersonProfileWindow { DataContext = vm };
            window.Show();
        }

        /// <summary>
        /// Starts the email verification view logic (OnActivatedAsync) and opens the window.
        /// </summary>
        private void InitializeEmailVerification()
        {
            _ = m_EmailVerificationViewModel.OnActivatedAsync();
            m_EmailVerificationWindow.Show();
        }

        /// <summary>
        /// Loads gallery images from the server and displays the gallery window.
        /// </summary>
        private void InitializeGallery()
        {
            _ = m_GalleryViewModel.LoadImagesAsync();
            m_GalleryWindow.Show();
        }

        private void ReConnect()
        {
            ReInitialize();
            m_DisconnectedWindow.Show();
        }

        private void ReInitialize()
        {
            UnsubscribeFromStateNotifiers();
            UnsubscribeFromDetailNotifiers();
            UnsubscribeFromNetworkEvents();
            InitializeKeyComponents();
            SubscribeToAllCriticalEvents();
        }

        private void InitializeKeyComponents()
        {
            m_NetworkFacade = new NetworkFacade();
            m_NetworkFacade.Connect();

            m_SharedImageStore = new SharedImageStore();
            m_StateNotifiers = new List<IStateNotifier>();
            m_DetailNotifiers = new List<IDetailNotifier<AdvancedPersonDataWithImage>>();
            m_UserSession = new UserSession();
            m_Mapper = new Mapper();
            m_GalleryService = new GalleryService(m_NetworkFacade, m_Mapper);

            // Create view models with their required dependencies
            m_LogInViewModel = new LogInViewModel(m_NetworkFacade, m_UserSession);
            m_SignUpViewModel = new SignUpViewModel(m_NetworkFacade, m_UserSession);
            m_FaceRecognitionViewModel = new FaceRecognitionViewModel(m_NetworkFacade, m_GalleryService, m_SharedImageStore, m_Mapper);
            m_CameraCaptureViewModel = new CameraCaptureViewModel(m_SharedImageStore);
            m_CaptchaViewModel = new CaptchaViewModel();
            m_EmailVerificationViewModel = new EmailVerificationViewModel(m_NetworkFacade, m_UserSession);
            m_GalleryViewModel = new GalleryViewModel(m_GalleryService, m_PendingGalleryImage, m_UserSession);
            m_ForgotPasswordViewModel = new ForgotPasswordViewModel(m_NetworkFacade);
            m_GeneralAttendanceViewModel = new GeneralAttendanceViewModel(m_NetworkFacade, m_Mapper);
            m_NavigationWindowViewModel = new NavigationWindowViewModel();

            // Create views and bind them to their respective DataContexts
            m_LogInWindow = new LogInWindow { DataContext = m_LogInViewModel };
            m_SignUpWindow = new SignUpWindow { DataContext = m_SignUpViewModel };
            m_FaceRecognitionWindow = new FaceRecoginitionWindow { DataContext = m_FaceRecognitionViewModel };
            m_CameraCaptureWindow = new CameraCaptureWindow { DataContext = m_CameraCaptureViewModel };
            m_CaptchaWindow = new CaptchaWindow { DataContext = m_CaptchaViewModel };
            m_EmailVerificationWindow = new EmailVerificationWindow { DataContext = m_EmailVerificationViewModel };
            m_GalleryWindow = new GalleryWindow { DataContext = m_GalleryViewModel };
            m_ForgotPasswordWindow = new ForgotPasswordWindow { DataContext = m_ForgotPasswordViewModel };
            m_GeneralAttendanceWindow = new GeneralAttendanceView { DataContext = m_GeneralAttendanceViewModel };
            m_NavigationWindow = new NavigationWindow { DataContext = m_NavigationWindowViewModel };
        }

        private void HandleTrigger(ApplicationTrigger trigger)
        {
            m_WindowNavigationSystem.Fire(trigger);
        }

        private void UnsubscribeFromStateNotifiers()
        {
            foreach (var notifier in m_StateNotifiers)
            {
                notifier.OnTriggerOccurred -= HandleTrigger;
            }
        }

        private void HandleDetailRequest(AdvancedPersonDataWithImage record)
        {
            m_PendingDetailsRecord = record;
            m_WindowNavigationSystem.Fire(ApplicationTrigger.ShowPersonDetails);
        }

        private void UnsubscribeFromDetailNotifiers()
        {
            foreach (var notifier in m_DetailNotifiers)
            {
                notifier.OnDetailRequested -= HandleDetailRequest;
            }
        }

        private void HandleServerDisconnect(string reason)
        {
            ClientLogger.LogInfo($"Server disconnected: {reason}");
            m_DisconnectedViewModel.ErrorMessage = reason;
            m_WindowNavigationSystem.Fire(ApplicationTrigger.UserDisconnected);
        }

        private void UnsubscribeFromNetworkEvents()
        {
            if (m_NetworkFacade is NetworkFacade facade)
            {
                facade.OnServerDisconnected -= HandleServerDisconnect;
            }
        }
    }
}
