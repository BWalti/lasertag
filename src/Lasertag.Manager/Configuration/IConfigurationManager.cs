using Lasertag.DomainModel.DomainEvents;

namespace Lasertag.Manager.Configuration;

public interface IConfigurationManager : IDomainManager<DomainModel.Configuration, ConfigurationState, IDomainEventBase>
{
}