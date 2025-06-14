/*
Poor Man's T-SQL Formatter - a small free Transact-SQL formatting 
library for .Net 2.0 and JS, written in C#. 
Copyright (C) 2017 Tao Klerks

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using WixToolset.Dtf.WindowsInstaller;

namespace PoorMansTSqlFormatterSSMSPackage.Setup.action
{
    public class SSMSPackageActions
    {
        const string SSMS21FILEKEY = "SSMS21FILE";
        const string APPLICATIONFOLDERKEY = "APPLICATIONFOLDER";
        const string CODEBASEENTRYPREFIX = "\"CodeBase\"=\"";
        const string PACKAGEFOLDERVARIABLE = "$PackageFolder$\\";


        [CustomAction]
        public static ActionResult PkgDefUpdateAction21(Session session)
        {
            return PkgDefUpdateAction(session, session.CustomActionData[SSMS21FILEKEY]);
        }

        public static ActionResult PkgDefUpdateAction(Session session, string ssmsExtensionsPath)
        {
            session.Log("Begin PkgDefUpdateAction - target path " + ssmsExtensionsPath);
            var extensionInstallFolder = session.CustomActionData[APPLICATIONFOLDERKEY];
            session.Log("InstallFolder detected as " + extensionInstallFolder);

            try
            {
                ReplacePackageCodebase(ssmsExtensionsPath, extensionInstallFolder);
            }
            catch (Exception ex)
            {
                session.Log("Failed PkgDefUpdateAction");
                session.Log("Exception: " + ex.ToString());
                return ActionResult.Failure;
            }

            session.Log("Finished PkgDefUpdateAction successfully");
            return ActionResult.Success;
        }

        static Regex matcherPattern = new Regex(Regex.Escape(CODEBASEENTRYPREFIX + PACKAGEFOLDERVARIABLE));
        private static void ReplacePackageCodebase(string ssmsPackageFile, string installFolder)
        {
            Encoding inputEncoding;
            string inputText;
            using (var inputReader = new StreamReader(ssmsPackageFile))
            {
                inputText = inputReader.ReadToEnd();
                inputEncoding = inputReader.CurrentEncoding;
            }
            string replacementValue = CODEBASEENTRYPREFIX + installFolder;
            File.WriteAllText(ssmsPackageFile, matcherPattern.Replace(inputText, replacementValue), inputEncoding);
        }
    }
}
