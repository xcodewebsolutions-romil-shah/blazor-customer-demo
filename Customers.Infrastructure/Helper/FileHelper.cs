using AutoMapper;
using CalculateTCIViaDLL;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.VisualBasic;
using Mysqlx.Connection;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Customers.Data.Domains;
using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;
using Twilio.TwiML.Voice;
using Xceed.Words.NET;
using static Google.Protobuf.Reflection.ExtensionRangeOptions.Types;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Customers.Infrastructure.Helper
{
    public static class FileHelper
    {
        public static async Task<string> GetAllTextFromImportFile(byte[] FileBytes, string FileExtension)
        {
            string textFromFile = string.Empty;
            switch (FileExtension.ToLower())
            {
                case ".txt":
                case ".text":
                    {
                        textFromFile = await GetAllTextFromTextFile(FileBytes);
                        break;
                    }
                case ".doc":
                case ".docx":
                    {
                        textFromFile = GetAllTextFromWordFile(FileBytes);
                        break;
                    }
                case ".pdf":
                    {
                        textFromFile = GetAllTextFromPDFFile(FileBytes);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return textFromFile;
        }
        private static async Task<string> GetAllTextFromTextFile(byte[] fileBytes)
        {
            try
            {
                using (System.IO.Stream stream = new MemoryStream(fileBytes))
                {
                    var reader = new StreamReader(stream);
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private static string GetAllTextFromWordFile(byte[] docFileBytes)
        {
            try
            {
                using (System.IO.Stream stream = new MemoryStream(docFileBytes))
                {
                    using (DocX document = DocX.Load(stream))
                    {
                        return document.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        private static string GetAllTextFromPDFFile(byte[] FileBytes)
        {
            try
            {
                string AllText = string.Empty;
                if (FileBytes != null && FileBytes.Length > 0)
                {
                    PdfReader pdfReader = new PdfReader(FileBytes);
                    for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        AllText += PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);
                    }
                    pdfReader.Close();
                }
                return AllText;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static string CleanText(string inputText)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in inputText)
            {
                if (((ch >= 65) && (ch <= 90)) || ((ch >= 97) && (ch <= 122)) || (ch == 32))
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append(Convert.ToChar(32));          // If not printable text...Add as space so as to preserve word spacing and such.
                }
            }
            return sb.ToString();
        }
        public static List<string> SplitStringTextIntoWords(string inputFileString)
        {
            List<string> wordsList = new List<string>();

            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

            string[] words = inputFileString.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                wordsList.Add(word.ToLower().Trim());
            }

            return wordsList;
        }
        public static List<WordCountList> GenerateWordOccurenceList(List<string> inputList)
        {
            List<WordCountList> outputList = new List<WordCountList>();
            var g = inputList.GroupBy(i => i).OrderByDescending(group => group.Count());

            foreach (var grp in g)
                outputList.Add(new WordCountList { Word = grp.Key, WordCount = grp.Count() });

            return outputList;
        }

        public static List<WordCountList> RemoveDuplicateWordEntries_WordCountList(List<WordCountList> inputList)
        {
            return inputList.Distinct().ToList();
        }

        public static List<WordCountList> RemoveSingleCharacterWordEntries_WordCountList(List<WordCountList> inputList)
        {
            List<WordCountList> outputList = new List<WordCountList>();
            foreach (WordCountList wcl in inputList)
            {
                if (wcl.Word.Length != 1)
                    outputList.Add(wcl);
            }
            return outputList;
        }
        public static List<WordCountList> RemovePluralsAndEndings_WordCountList(List<WordCountList> inputList)
        {
            List<WordCountList> outputList = new List<WordCountList>();

            string tempWordString = string.Empty;

            try
            {
                foreach (WordCountList wcl in inputList)
                {
                    WordCountList wclToAdd = new WordCountList();
                    if (wcl.Word.ToLower().EndsWith("ness"))
                    {
                        wclToAdd.Word = RemoveWordEnding(wcl.Word, "ness");
                        wclToAdd.WordCount = wcl.WordCount;
                        outputList.Add(wclToAdd);
                    }
                    else if (wcl.Word.ToLower().EndsWith("s"))
                    {
                        wclToAdd.Word = RemoveWordEnding(wcl.Word, "s");
                        wclToAdd.WordCount = wcl.WordCount;
                        outputList.Add(wclToAdd);
                    }
                    else if (wcl.Word.ToLower().EndsWith("ly"))
                    {
                        wclToAdd.Word = RemoveWordEnding(wcl.Word, "ly");
                        wclToAdd.WordCount = wcl.WordCount;
                        outputList.Add(wclToAdd);
                    }
                    else if (wcl.Word.ToLower().EndsWith("ing"))
                    {
                        wclToAdd.Word = RemoveWordEnding(wcl.Word, "ing");
                        wclToAdd.WordCount = wcl.WordCount;
                        outputList.Add(wclToAdd);
                    }
                    else
                    {
                        outputList.Add(wcl);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return outputList;
        }
        private static string RemoveWordEnding(string word, string ending)
        {
            string newWord = string.Empty;

            int lastIndex = word.LastIndexOf(ending);

            if (lastIndex > 0)
            {
                newWord = word.Remove(lastIndex);
            }
            else
            {
                // No change.
                newWord = word;
            }

            return newWord;
        }

        public static string ConvertWordCountListToString(List<WordCountList> inputWordCountList)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Convert List<string> to a string we can store in the database or a file.
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////
            List<string> stringListToString = new List<string>();
            string tempString = string.Empty;

            try
            {
                foreach (WordCountList wc in inputWordCountList)
                {
                    // Convert the Word and WordCount to a pipe delimited string and add it to the wordCountListToString.
                    tempString = wc.Word + "|" + wc.WordCount.ToString();
                    stringListToString.Add(tempString);
                }
            }
            catch (Exception ex)
            {
            }
            // Now use that "packed" string list to output to a string that we can store in the DB.

            string textWordCountListString = string.Empty;
            try
            {
                textWordCountListString = string.Join(Environment.NewLine, stringListToString);
            }
            catch (Exception ex)
            {
            }
            return textWordCountListString;
        }

        private static float CalculateWordPercentage(int sowWordCount, int totalWordCount)
        {
            float results = 0;

            try
            {
                results = (float)sowWordCount / (float)totalWordCount;
            }
            catch (Exception ex)
            {
            }
            return results;
        }

        private static float CalculateWordPercentageAbsolute(int sowWordCount, int proposalWordCount, int totalWordCount)
        {
            float resultsAbsolute = 0;
            float sowPercent = 0;
            float proposalPercent = 0;
            float resultsRaw = 0;

            try
            {
                sowPercent = (float)sowWordCount / (float)totalWordCount;

                proposalPercent = (float)proposalWordCount / (float)totalWordCount;

                resultsRaw = sowPercent - proposalPercent;

                resultsAbsolute = Math.Abs(resultsRaw);
            }
            catch (Exception ex)
            {
            }
            return resultsAbsolute;
        }

        public static List<AnalysisDetail> GetAnalysisDetails(string sowText, string proposalText)
        {
            var SOWWordCountList = new List<WordCountList>();
            var wordsCountList = sowText.Split("\r\n");
            foreach (var wordCount in wordsCountList)
            {
                var word = wordCount.Trim().Split("|");
                SOWWordCountList.Add(new WordCountList()
                {
                    Word = word[0],
                    WordCount = Convert.ToInt32(word[1])
                });
            }

            var VPWordCountList = new List<WordCountList>();
            var proposalwordsCountList = proposalText.Split("\r\n");
            foreach (var wordCount in proposalwordsCountList)
            {
                var word = wordCount.Trim().Split("|");
                VPWordCountList.Add(new WordCountList()
                {
                    Word = word[0],
                    WordCount = Convert.ToInt32(word[1])
                });
            }

            SOWWordCountList = SOWWordCountList.OrderByDescending(w => w.WordCount).ToList();
            VPWordCountList = VPWordCountList.OrderByDescending(w => w.WordCount).ToList();

            //var Top1SOWWord = SOWWordCountList[0].Word;
            //var Top2SOWWord = SOWWordCountList[1].Word;

            //var Top1ProposalWord = VPWordCountList[0].Word;
            //var Top2ProposalWord = VPWordCountList[1].Word;

            List<WordsInText> Top50WordsSOWList = new List<WordsInText>();

            int SOWListNumberOfEntries = 0;

            if (SOWWordCountList.Count > 50)
            {
                SOWListNumberOfEntries = 50;
            }
            else
            {
                SOWListNumberOfEntries = SOWWordCountList.Count;
            }

            for (int i = 0; i < SOWListNumberOfEntries; i++)
            {
                Top50WordsSOWList.Add(new WordsInText(i + 1, SOWWordCountList[i].Word, SOWWordCountList[i].WordCount));
            }

            List<WordsInText> Top50WordsProposalList = new List<WordsInText>();

            int ProposalListNumberOfEntries = 0;

            if (VPWordCountList.Count > 50)
            {
                ProposalListNumberOfEntries = 50;
            }
            else
            {
                ProposalListNumberOfEntries = VPWordCountList.Count;
            }

            for (int i = 0; i < ProposalListNumberOfEntries; i++)
            {
                Top50WordsProposalList.Add(new WordsInText(i + 1, VPWordCountList[i].Word, VPWordCountList[i].WordCount));
            }

            //string SOWTop20WordsFrequencyString = string.Empty;
            //string currentWord = string.Empty;
            //int currentWordCount = 0;

            //    for (int i = 0; i < SOWListNumberOfEntries; i++)
            //    {
            //        currentWord = Top20WordsSOWList[i].Word;
            //        currentWordCount = Top20WordsSOWList[i].Count;

            //        for (int wc = 0; wc < currentWordCount; wc++)
            //        {
            //            //SOWTop20WordsFrequencyString = SOWTop20WordsFrequencyString + currentWord + Environment.NewLine;

            //            SOWTop20WordsFrequencyString = SOWTop20WordsFrequencyString + " " + currentWord + " ";
            //        }

            //        SOWTop20WordsFrequencyString = SOWTop20WordsFrequencyString + Environment.NewLine;
            //    }

            //string ProposalTop20WordsFrequencyString = string.Empty;
            //currentWord = string.Empty;
            //currentWordCount = 0;

            //    for (int i = 0; i < ProposalListNumberOfEntries; i++)
            //    {
            //        currentWord = Top20WordsProposalList[i].Word;
            //        currentWordCount = Top20WordsProposalList[i].Count;

            //        for (int wc = 0; wc < currentWordCount; wc++)
            //        {
            //            //ProposalTop20WordsFrequencyString = ProposalTop20WordsFrequencyString + currentWord + Environment.NewLine;

            //            ProposalTop20WordsFrequencyString = ProposalTop20WordsFrequencyString + " " + currentWord + " ";
            //        }

            //        ProposalTop20WordsFrequencyString = ProposalTop20WordsFrequencyString + Environment.NewLine;
            //    }
            //    
            var wordsCountComparisonList = new List<WordCountComparisonList>();
            foreach (WordCountList wc in SOWWordCountList)
            {
                bool weFoundIt = false;
                var wordSOW = wc.Word;
                var countSOW = wc.WordCount;

                weFoundIt = false;

                // 1. If word is also present in the VP list...Add both with their respective word counts.
                foreach (WordCountList vpWC in VPWordCountList.ToArray())
                {
                    var wordVP = vpWC.Word;
                    var countVP = vpWC.WordCount;

                    if (wordVP == wordSOW)
                    {
                        // We have a match!

                        weFoundIt = true;

                        wordsCountComparisonList.Add(new WordCountComparisonList { Word = wordSOW, WordCountSOW = countSOW, WordCountVP = countVP });

                        // Remove this item from the VP WordCountList.
                        VPWordCountList.Remove(vpWC);
                    }
                }


                // 3. If word is NOT present in the VP list...Add it and set the VP word count to 0

                if (weFoundIt == false)
                {
                    // We did NOT find the word in the VPWordCountList...So...
                    // Add it as an orphaned item.

                    wordsCountComparisonList.Add(new WordCountComparisonList { Word = wordSOW, WordCountSOW = countSOW, WordCountVP = 0 });
                }
            }



            //if (SettingsClass.MainFormReference.checkEdit_IncludeZeroWords.Checked == true)
            //{
            //    // Go through the remaining VP word count list and add the remaining entries with a 0 for the VP word count.

            //    foreach (WordCountList vpWC in VPWordCountList.ToArray())
            //    {
            //        var wordVP = vpWC.Word;
            //        var countVP = vpWC.WordCount;

            //        wordsCountComparisonList.Add(new WordCountComparisonList { Word = wordVP, WordCountSOW = 0, WordCountVP = countVP });
            //    }
            //}

            List<AnalysisDetail> analysisDetails = new();
            int totalWordsCount = wordsCountComparisonList.Count;
            foreach (WordCountComparisonList wcl in wordsCountComparisonList)
            {
                analysisDetails.Add(new AnalysisDetail()
                {
                    Word = wcl.Word,
                    SOWCount = wcl.WordCountSOW,
                    SOWPercentage = CalculateWordPercentage(wcl.WordCountSOW, totalWordsCount) * 100,
                    ProposalCount = wcl.WordCountVP,
                    ProposalPercentage = CalculateWordPercentage(wcl.WordCountVP, totalWordsCount) * 100,
                    Delta = Math.Abs(wcl.WordCountVP - wcl.WordCountSOW),
                    DeltaPercentage = CalculateWordPercentageAbsolute(wcl.WordCountSOW, wcl.WordCountVP, totalWordsCount) * 100
                });
            }
            var txt = JsonSerializer.Serialize(analysisDetails);
            return analysisDetails.OrderByDescending(x => x.SOWCount).ToList();
        }


        public static OutputClass CalculateTCI(List<WordCountComparisonList> wordsCountComparisonList,
            ActiveRecordSOW activeSOW,
            ActiveRecordProposal activeProposal)
        {
            double TCI = 0;
            OutputClass outputClass = new OutputClass();
            try
            {
                        float Correlation = 0;
                float Correlation100 = 0;

                long SOWPageCount = 0;
                long ProposalPageCount = 0;

                long SOWParagraphCount = 0;
                long ProposalParagraphCount = 0;

                long characterCountSOW = 0;
                long characterCountProposal = 0;

                float SOWParagraphCharacterCount = 0;
                float ProposalParagraphCharacterCount = 0;

                long SOWWordCount = 0;
                long ProposalWordCount = 0;

                float SOWWordCountDividedBy10 = 0;
                float ProposalWordCountDividedBy10 = 0;

                float ArrayXY = 0;

                float CrossProduct = 0;
                float MCS = 0;

                int wordCountSOWNumber = 0;
                int wordCountProposalNumber = 0;

                var wordCountSOWList = new List<double>();
                var wordCountProposalList = new List<double>();

                var wordSOWList = new List<string>();
                var wordProposalList = new List<string>();

                List<int> intList = new List<int>();

                try
                {
                    foreach (WordCountComparisonList wordCount in wordsCountComparisonList)
                    {
                        wordCountSOWNumber = wordCount.WordCountSOW;

                        wordCountSOWList.Add((double)wordCountSOWNumber);


                        wordCountProposalNumber = wordCount.WordCountVP;

                        wordCountProposalList.Add((double)wordCountProposalNumber);
                    }
                }
                catch (Exception ex)
                {
                    return new();
                }
                try
                {
                    Correlation = (float)Correl1(wordCountSOWList, wordCountProposalList);
                }
                catch (Exception ex)
                {
                    return new();
                }
                try
                {
                    Correlation100 = Correlation * 100;
                }
                catch (Exception ex)
                {
                    return new OutputClass();
                }

                SOWPageCount = activeSOW.PagesCount ?? 0;
                ProposalPageCount = activeProposal.PagesCount ?? 0;

                SOWParagraphCount = activeSOW.ParagraphCount ?? 0;
                ProposalParagraphCount = activeProposal.ParagraphCount ?? 0;

                characterCountSOW = 0;

                try
                {
                    foreach (char character in activeSOW.CleanedText)
                    {
                        if (character <= 64)
                        {
                        }
                        else if ((character >= 65) || (character <= 90))        // A-Z
                        {
                            characterCountSOW++;
                        }
                        else if ((character >= 91) || (character <= 96))
                        {
                        }
                        else if ((character >= 97) || (character <= 122))       // a-z
                        {
                            characterCountSOW++;
                        }
                        else if (character >= 123)
                        {
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new();
                }

                characterCountProposal = 0;

                try
                {
                    foreach (char character in activeProposal.CleanedText)
                    {
                        if (character <= 64)
                        {
                        }
                        else if ((character >= 65) || (character <= 90))        // A-Z
                        {
                            characterCountProposal++;
                        }
                        else if ((character >= 91) || (character <= 96))
                        {
                        }
                        else if ((character >= 97) || (character <= 122))       // a-z
                        {
                            characterCountProposal++;
                        }
                        else if (character >= 123)
                        {
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new OutputClass();
                }

                try
                {
                    SOWParagraphCharacterCount = (float)characterCountSOW / SOWParagraphCount;
                    ProposalParagraphCharacterCount = (float)characterCountProposal / ProposalParagraphCount;
                }
                catch (Exception ex)
                {
                    return new OutputClass();
                }

                try
                {
                    SOWWordCountDividedBy10 = (float)SOWWordCount / 10;
                    ProposalWordCountDividedBy10 = (float)ProposalWordCount / 10;
                }
                catch (Exception ex)
                {
                    return new OutputClass();
                }

                long sowPagesCount = activeSOW.PagesCount ?? 0;
                long vpPagesCount = activeProposal.PagesCount ?? 0;


                TCIClass tci = new TCIClass();

                outputClass = tci.CalculateTCI(sowPagesCount, vpPagesCount, Correlation100, SOWParagraphCharacterCount, ProposalParagraphCharacterCount, SOWWordCountDividedBy10, ProposalWordCountDividedBy10);

                TCI = outputClass.TCI;

                ArrayXY = outputClass.ArrayXY;
                CrossProduct = outputClass.CrossProduct;
                MCS = outputClass.MCS;

                try
                {
                    var wordsPerPageSOW = (float)SOWWordCount / SOWPageCount;

                    var paragraphsPerPageSOW = (float)SOWParagraphCount / SOWPageCount;
                }
                catch (Exception ex)
                {
                    return new OutputClass();
                }

            }
            catch (Exception ex)
            {
                return new OutputClass();
            }

            return outputClass;
        }

        public static double Correl1(List<double> x, List<double> y)
        {
            //// // Log.Information("AnalyzeXtraForm.cs - Correl1() - Start...");

            Double stdX = 0;
            Double stdY = 0;
            Double covariance = 0;

            try
            {
                // See:  https://stackoverflow.com/questions/17447817/correlation-of-two-arrays-in-c-sharp

                // https://stackoverflow.com/questions/17447817/correlation-of-two-arrays-in-c-sharp

                if (x.Count != y.Count)
                {
                    return (double.NaN); //throw new ArgumentException("values must be the same length");
                }

                double sumX = 0;
                double sumX2 = 0;
                double sumY = 0;
                double sumY2 = 0;
                double sumXY = 0;

                int n = x.Count < y.Count ? x.Count : y.Count;

                for (int i = 0; i < n; ++i)
                {

                    Double xval = x[i];
                    Double yval = y[i];

                    sumX += xval;
                    sumX2 += xval * xval;
                    sumY += yval;
                    sumY2 += yval * yval;
                    sumXY += xval * yval;
                }

                stdX = Math.Sqrt(sumX2 / n - sumX * sumX / n / n);
                stdY = Math.Sqrt(sumY2 / n - sumY * sumY / n / n);
                covariance = (sumXY / n - sumX * sumY / n / n);
            }
            catch (Exception ex)
            {
                //// // Log.Error("Correl1() Error: " + ex.Message);

                //AnalysisFatalError();

                return 0.0;
            }

            //// // Log.Information("AnalyzeXtraForm.cs - Correl1() - Completed.");


            return covariance / stdX / stdY;
        }
    }

    public class WordCountList
    {
        public string Word = string.Empty;

        public int WordCount = 0;

        public override string ToString()
        {
            return Word + " | " + WordCount;
        }
    }

    public class WordCountComparisonList
    {
        public string Word = string.Empty;

        public int WordCountSOW = 0;

        public int WordCountVP = 0;
    }
    public class WordsInText
    {
        public WordsInText(int topNumber, string word, int count)
        {
            TopNumber = topNumber;
            Word = word;
            Count = count;
        }

        public int TopNumber = 0;

        public string Word = string.Empty;

        public int Count = 0;
    }
}
