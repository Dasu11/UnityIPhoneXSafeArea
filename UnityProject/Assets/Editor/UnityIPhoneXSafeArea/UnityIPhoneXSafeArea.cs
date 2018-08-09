#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class UnityIPhoneXSafeArea {

    //Default potrait view
    private const float _HeadArea = 45.0f;
    private const float _FootArea = 45.0f;
    private const float _RightArea = 15.0f;
    private const float _LeftArea = 15.0f;

    private static Color _HeadAreaColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    private static Color _FootAreaColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    private static Color _RightAreaColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    private static Color _LeftAreaColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path) {
        if (UnityEditor.PlayerSettings.statusBarHidden) {
            SetBaseViewController(path);
        }

        string _UnityAppControllerFile = Path.Combine(path, "Classes/UnityAppController.mm");
        string _UnityAppControllerFileContent = File.ReadAllText(_UnityAppControllerFile);
        string _OldTextInUnityAppControllerFile = "    _window         = [[UIWindow alloc] initWithFrame: [UIScreen mainScreen].bounds];";
        string _NewTextToReplace = "    CGRect rect = [UIScreen mainScreen].bounds;\n" +
                           "    if (rect.size.width / rect.size.height > 2.15f) {{\n" +
                           "        rect.origin.x = {0:0.0#####}f;\n" +
                           "        rect.size.width -= ({0:0.0#####}f + {1:0.0#####}f);\n" +
                           "        rect.origin.y = {2:0.0#####}f;\n" +
                           "        rect.size.height -= ({2:0.0#####}f + {3:0.0#####}f);\n" +
                           "        _window = [[UIWindow alloc] initWithFrame: rect];\n" +

                           "        UIView *leftView = [[UIView alloc] initWithFrame:CGRectMake(-{0:0.0#####}f, 0, {0:0.0#####}f, rect.size.height)];\n" +
                           "        [leftView setBackgroundColor:[UIColor colorWithRed:{4:0.0#####}f green:{5:0.0#####}f blue:{6:0.0#####}f alpha:{7:0.0#####}f]];\n"+
                           "        [_window addSubview:leftView];\n" +
                          
                           "        UIView *rightView = [[UIView alloc] initWithFrame:CGRectMake(rect.size.width, 0, rect.size.width, {1:0.0#####}f)];\n" +
                           "        [rightView setBackgroundColor:[UIColor colorWithRed:{8:0.0#####}f green:{9:0.0#####}f blue:{10:0.0#####}f alpha:{11:0.0#####}f]];\n"+
                           "        [_window addSubview:rightView];\n" +            
                         
                           "        UIView *headerView = [[UIView alloc] initWithFrame:CGRectMake(0, -{2:0.0#####}f, rect.size.width, {2:0.0#####}f)];\n"+
                           "        [headerView setBackgroundColor:[UIColor colorWithRed:{12:0.0#####}f green:{13:0.0#####}f blue:{14:0.0#####}f alpha:{15:0.0#####}f]];\n"+
                           "        [_window addSubview:headerView];\n"+

                           "        UIView *footerView = [[UIView alloc] initWithFrame:CGRectMake(0, rect.size.height,rect.size.width, {3:0.0#####}f)];\n"+
                           "        [footerView setBackgroundColor:[UIColor colorWithRed:{16:0.0#####}f green:{17:0.0#####}f blue:{18:0.0#####}f alpha:{19:0.0#####}f]];\n"+
                           "        [_window addSubview:footerView];\n"+

                           "    }} else {{\n" +
                           "        _window = [[UIWindow alloc] initWithFrame: rect];\n" +
                           "    }}\n";

                            _NewTextToReplace = string.Format(_NewTextToReplace, _HeadArea, _FootArea,_RightArea,_LeftArea,
                                _HeadAreaColor.r, _HeadAreaColor.g, _HeadAreaColor.b, _HeadAreaColor.a,
                                _FootAreaColor.r, _FootAreaColor.g, _FootAreaColor.b, _FootAreaColor.a,
                                _RightAreaColor.r, _RightAreaColor.g, _RightAreaColor.b, _RightAreaColor.a,
                                _LeftAreaColor.r, _LeftAreaColor.g, _LeftAreaColor.b, _LeftAreaColor.a);
                            _UnityAppControllerFileContent = _UnityAppControllerFileContent.Replace(_OldTextInUnityAppControllerFile, _NewTextToReplace);

                            File.WriteAllText(_UnityAppControllerFile, _UnityAppControllerFileContent);


            SetUnityView(path);

    }

    void SetBaseViewController(string path)
    {
        var _InfoPlistPath = Path.Combine(path, "Info.plist");
        var _InfoPlist = new PlistDocument();
        _InfoPlist.ReadFromFile(_InfoPlistPath);
        _InfoPlist.root.SetBoolean("UIStatusBarHidden", false);
        _InfoPlist.WriteToFile(_InfoPlistPath);

        string _UnityViewControllerBaseFile = Path.Combine(path, "Classes/UI/UnityViewControllerBaseiOS.mm");
        if (!File.Exists(_UnityViewControllerBaseFile))
        {
            _UnityViewControllerBaseFile = Path.Combine(path, "Classes/UI/UnityViewControllerBase+iOS.mm");
        }
        string _UnityViewControllerBaseFileContent = File.ReadAllText(_UnityViewControllerBaseFile);
        string _OldTextInFile = "    return _PrefersStatusBarHidden;";
        string _NewTextToReplace = "    CGSize size = [UIScreen mainScreen].bounds.size;\n" +
                           "    if (size.width / size.height > 2.15f) {\n" +
                           "        return NO;\n" +
                           "    } else {\n" +
                           "        return YES;\n" +
                           "    }";
        _UnityViewControllerBaseFileContent = _UnityViewControllerBaseFileContent.Replace(_OldTextInFile, _NewTextToReplace);
        File.WriteAllText(_UnityViewControllerBaseFile, _UnityViewControllerBaseFileContent);
    }

    void SetUnityView(string path)
    {
        string _UnityViewPath = Path.Combine(path, "Classes/UI/UnityView.mm");
        string _UnityViewContent = File.ReadAllText(_UnityViewPath);
        string _OldTextInFile = "    CGRect  frame   = [UIScreen mainScreen].bounds;";
        string _NewTextToReplace = "    CGRect frame = [UIScreen mainScreen].bounds;\n" +
                          "    if (frame.size.width / frame.size.height > 2.15f) {{\n" +
                          "        frame.size.width -= " + (_HeadArea + _FootArea) + "f;\n" +
                          "        frame.size.height -= "+ (_RightArea + _LeftArea)+ "f;\n" +
                          "    }}";

        _UnityViewContent = _UnityViewContent.Replace(_OldTextInFile, _NewTextToReplace);
        File.WriteAllText(_UnityViewPath, _UnityViewContent);
    }
}
#endif
