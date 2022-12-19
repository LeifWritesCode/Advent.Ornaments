using System.Diagnostics;

namespace Ornaments.Text;

internal interface IOpticalCharacterRecognitionStategy
{
    bool TryRead(string[] lines, out string text);
}

public enum OcrStrategy
{
    FourBySixWithSpace,
}

public class OpticalCharacterRecognition : IOpticalCharacterRecognitionStategy
{
    private readonly IOpticalCharacterRecognitionStategy opticalCharacterRecognitionStategy;

    private OpticalCharacterRecognition(IOpticalCharacterRecognitionStategy opticalCharacterRecognitionStategy)
    {
        this.opticalCharacterRecognitionStategy = opticalCharacterRecognitionStategy;
    }

    public bool TryRead(string[] lines, out string text)
    {
        return opticalCharacterRecognitionStategy.TryRead(lines, out text);
    }

    public static bool TryRead(OcrStrategy ocrStrategy, string[] lines, out string text)
    {
        var ocr = new OpticalCharacterRecognition(ocrStrategy switch
        {
            OcrStrategy.FourBySixWithSpace => new OcrStrategy4x6(),
            _ => throw new UnreachableException()
        });
        return ocr.TryRead(lines, out text);
    }
}

public class OcrStrategy4x6 : IOpticalCharacterRecognitionStategy
{
    private const int charWidth = 5;
    private const int charHeight = 6;
    private readonly IDictionary<string, char> characterMap = new Dictionary<string, char>()
    {
        {
            """
            .##..
            #..#.
            #..#.
            ####.
            #..#.
            #..#.
            """,
            'A'
        }
    };

    internal OcrStrategy4x6() { }

    public bool TryRead(string[] lines, out string text)
    {
        text = string.Empty;
        if (lines is null || lines.Length != charHeight)
            return false;

        var charactersInArray = lines.First().Length / charWidth;
        for (var i = 0; i < charactersInArray; i++)
        {
            var candidate = string.Join(string.Empty, lines.Select(x => x[i..(i + charWidth)]));
            if (!characterMap.TryGetValue(candidate, out char value))
            {
                return false;
            }
            text += value;
        }
        return true;
    }
}
