namespace BlazorAPI.Interfaces.Autenticacao
{
    public interface IAutenticacao
    {
        public string GenerateToken(string _email, string _senha);
    }
}