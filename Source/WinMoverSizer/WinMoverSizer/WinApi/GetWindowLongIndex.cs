namespace WinMoverSizer.WinApi;

// https://www.pinvoke.net/default.aspx/user32.getwindowlong
public enum GetWindowLongIndex
{
   GWL_WNDPROC =    (-4),
   GWL_HINSTANCE =  (-6),
   GWL_HWNDPARENT = (-8),
   GWL_STYLE =      (-16),
   GWL_EXSTYLE =    (-20),
   GWL_USERDATA =   (-21),
   GWL_ID =     (-12)
}
