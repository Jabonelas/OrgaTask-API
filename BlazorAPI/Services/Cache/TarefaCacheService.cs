using BlazorAPI.DTOs;
using BlazorAPI.DTOs.Tarefa;
using BlazorAPI.Interfaces.Service.Cache;
using BlazorAPI.Interfaces.Service.Tarefa;
using BlazorAPI.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BlazorAPI.Services.Cache
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

        public async Task<List<TarefaConsultaDTO>> GetListaTarefasCacheAsync(int _idUsuario)
        {
            var cacheKey = GetCacheKey(_idUsuario);
            var cachedData = await cache.GetStringAsync(cacheKey);

            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<List<TarefaConsultaDTO>>(cachedData);
            }

            var tarefas = await tarefaService.ListaTarefasIdAsync(_idUsuario);

            await AtualziarTarefasCacheAsync(_idUsuario, tarefas);

            return tarefas;
        }

        public async Task AtualziarTarefasCacheAsync(int _idUsuario, List<TarefaConsultaDTO> _tarefas)
        {
            var cacheKey = GetCacheKey(_idUsuario);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheDuration
            };

            await cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(_tarefas),
                options
            );
        }

        public async Task InvalidarCache(int _idUsuario)
        {
            var cacheKey = GetCacheKey(_idUsuario);
            await cache.RemoveAsync(cacheKey);
        }

        private static string GetCacheKey(int _idUsuario) => $"tarefas_user_{_idUsuario}";
    }
}