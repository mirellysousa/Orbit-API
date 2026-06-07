using Orbit.Api.Models;

namespace Orbit.Api.Services;

public interface IDecisionEngineService
{
    Task<CheckupMissao?> AvaliarMissaoAsync(int missaoId, CancellationToken cancellationToken);
}
