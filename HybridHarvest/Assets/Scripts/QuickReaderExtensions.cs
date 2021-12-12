using CI.QuickSave;

public class QSReader
{
    public static QuickSaveReader Create(string root)
    {
        QuickSaveReader reader;
        try
        {
            reader = QuickSaveReader.Create(root);
        }
        catch (QuickSaveException)
        {
            var writer = QuickSaveWriter.Create(root);
            writer.Commit();
            reader = QuickSaveReader.Create(root);
        }

        return reader;
    }
}
