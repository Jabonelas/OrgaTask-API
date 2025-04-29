namespace BlazorAPI.Interfaces.Autenticacao
{
    public interface IAutenticacao
    {
        public string GenerateToken(int _idUsuario, string _email);
    }
}