using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

public class FileManager
{
    public static void renameFile(string filename, string FileNameWithoutExtension)
    {
        if (File.Exists(filename))
        {
            string destFileName = filename.Replace(GetFileNameWithoutExtension(filename), FileNameWithoutExtension);
            try
            {
                File.Copy(filename, destFileName);
            }
            catch
            {
                return;
            }
        }
        try
        {
            new FileInfo(filename).Attributes = FileAttributes.Normal;
            File.Delete(filename);
        }
        catch
        {
        }
    }

    public static void deleteFile(string filename)
    {
        try
        {
            if (File.Exists(filename))
            {
                try
                {
                    new FileInfo(filename).Attributes = FileAttributes.Normal;
                    File.Delete(filename);
                    return;
                }
                catch
                {
                    return;
                }
            }
        }
        catch
        {
        }
    }

    public static void DeleteDirectory(string fullPath)
    {
        try
        {
            if (!Directory.Exists(fullPath))
            {
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath)
            {
                Attributes = FileAttributes.Normal
            };
            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos("*", SearchOption.AllDirectories);
            FileSystemInfo[] array = fileSystemInfos;
            foreach (FileSystemInfo fileSystemInfo in array)
            {
                if (fileSystemInfo is FileInfo)
                {
                    fileSystemInfo.Attributes = FileAttributes.Normal;
                }
            }
            Thread.Sleep(100);
            directoryInfo.Delete(recursive: true);
        }
        catch
        {
        }
    }

    public static bool FileExist(string filename)
    {
        if (File.Exists(filename))
        {
            return true;
        }
        return false;
    }

    public static bool DirectoryExist(string Directoryname)
    {
        if (Directory.Exists(Directoryname))
        {
            return true;
        }
        return false;
    }

    public static string GetFileName(string FilePath)
    {
        return Path.GetFileName(FilePath);
    }

    public static string GetFileNameWithoutExtension(string FilePath)
    {
        return Path.GetFileNameWithoutExtension(FilePath);
    }

    public static string GetExtension(string FilePath)
    {
        return Path.GetExtension(FilePath);
    }

    public static string GetFileLength(string FilePath)
    {
        FileInfo fileInfo = new FileInfo(FilePath);
        return fileInfo.Length.ToString();
    }

    public static void FileCopy(string source, string target)
    {
        if (File.Exists(source))
        {
            try
            {
                File.Copy(source, target);
            }
            catch
            {
            }
        }
    }

    public static void FileMove(string source, string target)
    {
        if (File.Exists(target))
        {
            try
            {
                new FileInfo(target).Attributes = FileAttributes.Normal;
                File.Delete(target);
            }
            catch
            {
                modCommon.Show("No se ha podido eliminar el mod en el directorio:" + Environment.NewLine + target);
            }
        }
        if (File.Exists(source))
        {
            try
            {
                File.Copy(source, target, overwrite: true);
            }
            catch
            {
                modCommon.Show("No se ha podido copiar el mod al directorio:" + Environment.NewLine + target);
            }
        }
        try
        {
            new FileInfo(source).Attributes = FileAttributes.Normal;
            File.Delete(source);
        }
        catch
        {
        }
    }

    internal static void HideDirectory(string Path)
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);
            directoryInfo.Attributes = FileAttributes.Hidden;
        }
        catch
        {
        }
    }

    internal static void HideFile(string file)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(file);
            fileInfo.Attributes = FileAttributes.Hidden;
        }
        catch
        {
        }
    }

    public static bool IsFolderUserAccessible(string path)
    {
        try
        {
            AuthorizationRuleCollection accessRules = new DirectoryInfo(path).GetAccessControl().GetAccessRules(includeExplicit: true, includeInherited: true, typeof(NTAccount));
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            foreach (AuthorizationRule item in accessRules)
            {
                if (!(item is FileSystemAccessRule fileSystemAccessRule) || (fileSystemAccessRule.FileSystemRights & FileSystemRights.WriteData) <= (FileSystemRights)0)
                {
                    continue;
                }
                try
                {
                    string value = item.IdentityReference.Translate(typeof(NTAccount)).Value;
                    IdentityReference identityReference = item.IdentityReference.Translate(typeof(SecurityIdentifier));
                    if (value.Equals(current.Name) || identityReference == securityIdentifier)
                    {
                        return true;
                    }
                }
                catch (IdentityNotMappedException)
                {
                }
            }
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static Encoding GetEncoding(string filename)
    {
        byte[] array = new byte[4];
        using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
        {
            fileStream.Read(array, 0, 4);
        }
        if (array[0] == 43 && array[1] == 47 && array[2] == 118)
        {
            return Encoding.UTF7;
        }
        if (array[0] == 239 && array[1] == 187 && array[2] == 191)
        {
            return Encoding.UTF8;
        }
        if (array[0] == byte.MaxValue && array[1] == 254)
        {
            return Encoding.Unicode;
        }
        if (array[0] == 254 && array[1] == byte.MaxValue)
        {
            return Encoding.BigEndianUnicode;
        }
        if (array[0] == 0 && array[1] == 0 && array[2] == 254 && array[3] == byte.MaxValue)
        {
            return Encoding.UTF32;
        }
        return Encoding.ASCII;
    }

    public static void MakeFolderUserAccessible(string directoryName)
    {
        ModifyFolderUsersAndPermissions(directoryName, WindowsIdentity.GetCurrent().Name, FileSystemRights.FullControl, AccessControlType.Allow);
    }

    public static void MakeFolderAllUsersAccessible(string directoryName)
    {
        try
        {
            string path = $"WinNT://{Environment.MachineName},computer";
            using DirectoryEntry directoryEntry = new DirectoryEntry(path);
            IEnumerable<string> enumerable = from DirectoryEntry dirchild in directoryEntry.Children
                                             where dirchild.SchemaClassName == "User"
                                             select dirchild.Name;
            foreach (string item in enumerable)
            {
                ModifyFolderUsersAndPermissions(directoryName, item, FileSystemRights.FullControl, AccessControlType.Allow);
            }
        }
        catch
        {
        }
    }

    private static void ModifyFolderUsersAndPermissions(string directoryName, string userAccount, FileSystemRights userRights, AccessControlType accessType)
    {
        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
            DirectorySecurity accessControl = directoryInfo.GetAccessControl();
            accessControl.AddAccessRule(new FileSystemAccessRule(userAccount, userRights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, accessType));
            directoryInfo.SetAccessControl(accessControl);
        }
        catch
        {
        }
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr ILCreateFromPathW(string pszPath);

    [DllImport("shell32.dll")]
    private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, int cild, IntPtr apidl, int dwFlags);

    [DllImport("shell32.dll")]
    private static extern void ILFree(IntPtr pidl);

    public static void OpenFolderAndSelectFile(string filePath)
    {
        if (filePath != null)
        {
            IntPtr intPtr = ILCreateFromPathW(filePath);
            SHOpenFolderAndSelectItems(intPtr, 0, IntPtr.Zero, 0);
            ILFree(intPtr);
        }
    }
}
