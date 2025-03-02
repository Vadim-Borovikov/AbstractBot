using GryphonUtilities.Save;

namespace AbstractBot.Interfaces.Modules.Context;

public interface IUserFinalData<out TUserFinalData, TUserSaveData> : IFinalData<TUserFinalData, TUserSaveData>
    where TUserFinalData : class, IUserFinalData<TUserFinalData, TUserSaveData>, new()
    where TUserSaveData : class, IUserSaveData, new()
{
} 