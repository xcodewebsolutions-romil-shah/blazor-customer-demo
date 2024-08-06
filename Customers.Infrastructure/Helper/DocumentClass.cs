using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Helper
{
    public static class DocumentClass
    {
        public static int CountWordInTextString(string inputString)
        {
            int num = 0;
            bool wasInaWord = true; ;
            if (string.IsNullOrEmpty(inputString))
                return num;

            try
            {
                for (int i = 0; i < inputString.Length; i++)
                {
                    if (i != 0)
                    {
                        if (inputString[i] == ' ' && inputString[i - 1] != ' ')
                        {
                            num++;
                            wasInaWord = false;
                        }
                    }
                    if (inputString[i] != ' ')
                    {
                        wasInaWord = true;
                    }
                }
                if (wasInaWord)
                    num++;
            }
            catch (Exception ex)
            {
            }
            return num;
        }
    }
}
