using DlibDotNet;

public class FaceComparer
{
    public bool AreSamePerson(Matrix<float> embedding1, Matrix<float> embedding2, double threshold = 0.6)
    {
        using (var diff = embedding1 - embedding2)
        {
            double distance = Dlib.Length(diff);
            return distance < threshold;
        }
    }
}