private static void splitAndTrim()
        {
            string str = @"Microsoft.EnterpriseManagement.Utility.WorkflowExpansion.dll E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\out\placefiled,E:\1ES.SCOMCPY\SystemCenter\Migration\SCOM\out\placefiledTest_Dependency\bin\";
            string dll;
            List<string> targetPaths = new List<string>();
            string[] separators = { ".dll" };
            string[] str1 = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            bool flag = true;
            foreach (var word in str1)
            {
                if (flag)
                {
                    Console.WriteLine("DLL");
                    Console.WriteLine(word);
                    dll = word;
                    flag = false;
                }
                else
                {
                    Console.WriteLine("Target Path/Paths");
                    targetPaths.Add(word);
                    Console.WriteLine("........");

                    string[] separators2 = { "," };
                    string[] str2 = word.Split(separators2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in str2.ToArray())
                    {
                        Console.WriteLine(item.Trim());
                    }

                }

            }

        }
