namespace WinMoverSizer.CoreApplication;

public interface IApplicationProxy
{
   void Exit();
}

public class ApplicationProxy : IApplicationProxy
{
   private readonly App _app;


   public ApplicationProxy(App app)
   {
      _app = app;
   }

   public void Exit()
   {
      _app.Shutdown();
   }
}
