using System;

namespace Artemis.UI.Services.Updating;

public interface IUpdateNotificationProvider
{
    void ShowNotification(Guid releaseId, string releaseVersion);
    void ShowInstalledNotification(string installedVersion);
}