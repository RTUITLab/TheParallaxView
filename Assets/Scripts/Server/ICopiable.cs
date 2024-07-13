public interface ICopiable<T> where T : class
{
    void Copy(T obj);
}
