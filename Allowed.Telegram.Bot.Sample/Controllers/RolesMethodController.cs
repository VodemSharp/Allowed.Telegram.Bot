namespace Allowed.Telegram.Bot.Sample.Controllers;

// [BotName("Sample")]
// public class RolesMethodController : CommandController<int>
// {
//     [Role("admin")]
//     [Command("admin_role_method")]
//     public async Task RoleControllerMethod(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id,
//             "You have permission for this command in method!");
//     }
//
//     [Role("manager")]
//     [Command("manager_role_method")]
//     public async Task AnotherRoleControllerMethod(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id,
//             "You have permission for this command in method!");
//     }
//
//     [Role("admin")]
//     [Role("manager")]
//     [Command("union_role_method")]
//     public async Task UnionRoleControllerMethod(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id,
//             "You have permission for this command in method!");
//     }
//
//     [Command("no_admin_role_method")]
//     public async Task NoRoleControllerMethod(MessageData data)
//     {
//         await data.Client.SendTextMessageAsync(data.Message.From!.Id,
//             "You have permission for this command in method!");
//     }
// }