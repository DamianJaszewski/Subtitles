// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

string path = @"C:\Users\oke\Downloads\napisydofilmu.srt";

ReadSubtitiles(path);

void ReadSubtitiles(string path)
{
    if (!File.Exists(path)){
        Console.WriteLine("Path is not correct."); 
        return;
    }

    string outputFile = "output.srt";
    string outputFile2 = "output2.srt";
    TimeSpan shiftTime = TimeSpan.FromMilliseconds(5880);
    Regex timeRegex = new Regex(@"(\d{2}):(\d{2}):(\d{2}),(\d{3})");
    var subtitlesToWrite = new List<string>();
    var subtitlesToWriteSecond = new List<string>();
    bool firstFileFlag = true;
    int numberFirstFile = 1;
    int numberSecondFile = 1;

    // Odczytanie zawartości pliku .SRT
    string[] lines = File.ReadAllLines(path);

    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i];

        if (int.TryParse(line, out int result))
        {
            if (firstFileFlag)
            {
                subtitlesToWrite.Add(numberFirstFile.ToString());
                numberFirstFile++;
            }
            else
            {
                subtitlesToWriteSecond.Add(numberSecondFile.ToString());
                numberSecondFile++;
            }
        }
        else if (timeRegex.IsMatch(line))
        {
            string[] timeParts = line.Split(" --> ");

            // Parsuj czasy początkowy i końcowy
            TimeSpan startTime = TimeSpan.ParseExact(timeParts[0], @"hh\:mm\:ss\,fff", null);
            TimeSpan endTime = TimeSpan.ParseExact(timeParts[1], @"hh\:mm\:ss\,fff", null);

            // Dodaj 5880 milisekund do godzin
            startTime = startTime.Add(shiftTime);
            endTime = endTime.Add(shiftTime);

            // Utwórz ponownie ciąg znaków z nowymi czasami
            string newTimeString = $"{startTime:hh\\:mm\\:ss\\,fff} --> {endTime:hh\\:mm\\:ss\\,fff}";

            if (startTime.Milliseconds != 0)
            {
                firstFileFlag = true;
                subtitlesToWrite.Add(newTimeString);
            }
            else
            {
                firstFileFlag = false;
                subtitlesToWriteSecond.Add(newTimeString);
            }
        }
        else
        {
            if (firstFileFlag)
            {
                subtitlesToWrite.Add(line);
            }
            else subtitlesToWriteSecond.Add(line);
        }
    }

    File.WriteAllLines(outputFile, subtitlesToWrite);
    File.WriteAllLines(outputFile2, subtitlesToWriteSecond);

    Console.WriteLine("Done!");
}


