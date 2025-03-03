public interface ITransitable
{
      bool IsInTransition { get; }
      public void BeginTransition();
     
      public void EndTransition();
    
}
   
