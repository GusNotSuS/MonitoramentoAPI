public class MonitoramentoService
{
    public async Task<(bool sucesso, string erro)> VerificarAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            var resposta = await client.GetAsync(url);
            return (resposta.IsSuccessStatusCode, resposta.IsSuccessStatusCode ? "" : resposta.ReasonPhrase);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
