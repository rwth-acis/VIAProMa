# Deep Links
## Create your own Deep Links
With the Deep Link Bootstrapper a Deep Linking Service is registered in VIAProMa. To make use and react to DeepLinks attributes are assigned to the functions you want to react. If you want your function to react to `i5://test` style deep links you have to add the attribute `[DeepLink("test)]`. You can also individually specify the scheme and path like this `[DeepLink(scheme: "i5", path: "test")]`. To get the deep link itself you need to add a parameter to your function: `void func(i5.Toolkit.Core.DeepLinkAPI.DeepLinkArgs args)`. 
If the function you want to react is not in the Bootstrapper, you need to add a listener to the object `i5.Toolkit.Core.DeepLinkAPI.DeepLinkingService.AddDeepLinkListener(System.Object)`.
## Invite Deep Links
Invite Deep Links have a format of `i5://invite?roomName=name` with `name` being the room name the user wants to join. The invite links are generated and can be displayed and copied in the room options menu. Options for sharing with e-mail or social media are coming soon.
With VIAProMa build and installed on the divice, pasting the link into a browser should give the option to open the link in VIAProMa. 
## Support
Currently only Universal Windows Platforms are supported. Android and iOS are comming soon. 