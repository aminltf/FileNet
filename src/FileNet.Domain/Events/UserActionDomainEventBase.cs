namespace FileNet.Domain.Events;

/// <summary>
/// Optional base to carry the user/system actor who caused the domain change.
/// Keeps Domain decoupled from any Identity implementation.
/// </summary>
public abstract record UserActionDomainEventBase(Guid? ActorId) : DomainEventBase;
