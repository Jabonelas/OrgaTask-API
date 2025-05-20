using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Service.Cache;
using BlazorAPI.Interfaces.Service.Tarefa;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BlazorAPI.Services
{
    public class TarefaCacheService : ITarefaCacheService
    {
        private readonly IDistributedCache cache;
        private readonly ITarefaService tarefaService;
        private readonly TimeSpan cacheDuration = TimeSpan.FromMinutes(10);

        public TarefaCacheService(IDistributedCache _cache, ITarefaService _tarefaService)
        {
            cache = _cache;
            tarefaService = _tarefaService;
        }

        public async Task<List<TarefaConsultaDTO>> GetTarefasCacheAsync(int usuarioId)
        {
            var cacheKey = GetCacheKey(usuarioId);
            var cachedData = await cache.GetStringAsync(cacheKey);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<List<TarefaConsultaDTO>>(cachedData);
            }

            var tarefas = await tarefaService.ListaTarefasIdAsync(usuarioId);

            await AtualziarTarefasCacheAsync(usuarioId, tarefas);

            return tarefas;
        }

        public async Task AtualziarTarefasCacheAsync(int usuarioId, List<TarefaConsultaDTO> tarefas)
        {
            var cacheKey = GetCacheKey(usuarioId);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };

            await cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(tarefas),
                options
            );
        }

        public async Task InvalidateCacheAsync(int usuarioId)
        {
            var cacheKey = GetCacheKey(usuarioId);
            await cache.RemoveAsync(cacheKey);
        }

        private static string GetCacheKey(int usuarioId) => $"tarefas_user_{usuarioId}";
    }
}