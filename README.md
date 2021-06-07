## **环境**

ASP.NET Core 3.1 MVC

IdentityServer4(4.1.2)

## 安装Nuget包

```C#
Install-Package IdentityServer4

Install-Package IdentityServer4.AspNetIdentity

Install-Package IdentityServer4.EntityFramework

Install-Package Microsoft.EntityFrameworkCore

Install-Package Microsoft.EntityFrameworkCore.SqlServer

Install-Package Microsoft.EntityFrameworkCore.Tools

Install-Package Pomelo.EntityFrameworkCore.MySql

Install-Package Newtonsoft.Json

Install-Package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation -Version 3.1.15
```



## 概念

#### 1、身份认证服务器（IdentityServer）

IdentityServer 是基于OpenID Connect协议标准的身份认证和授权程序，它实现了OpenID Connect 和 OAuth 2.0 协议。

IdentityServer有许多功能：

- 保护你的资源
- 使用本地帐户或通过外部身份提供程序对用户进行身份验证
- 提供会话管理和单点登录
- 管理和验证客户机
- 向客户发出标识和访问令牌
- 验证令牌

#### 2、Scope（ApiScopes）

scope结合策略授权来限制访问权限

#### 3、资源（Resources）

资源就是你想用IdentityServer保护的，可以是用户的身份数据或者API资源。

用户的身份信息实际由一组claim组成，例如姓名或者邮件都会包含在身份信息中（将来通过IdentityServer校验后都会返回给被调用的客户端）。

#### 4、客户端（Client）

客户端就是从IdentityServer请求令牌的软件（你可以理解为一个app即可），既可以通过身份认证令牌来验证识别用户身份，又可以通过授权令牌来访问服务端的资源。

#### 5、用户（User）

用户是使用已注册的客户端（指在id4中已经注册）访问资源的人

#### 6、访问令牌（Access Token）

访问令牌允许访问API资源，客户端请求访问令牌并将其转发到API， 访问令牌包含有关客户端和用户的信息（如果存在）， API使用该信息来授权访问其数据。

![avatar](/Imgs/GetToken.png)

#### 7、身份令牌（Id Token）

OIDC对OAuth2最主要的扩展就是提供了ID Token。ID Token是一个安全令牌，是一个授权服务器提供的包含用户信息（由一组Cliams构成以及其他辅助的Cliams）的JWT格式的数据结构。

OIDC在这个基础上提供了ID Token来解决第三方客户端标识用户身份认证的问题。OIDC的核心在于在OAuth2的授权流程中，一并提供用户的身份认证信息（ID Token）给到第三方客户端，ID Token使用JWT格式来包装，得益于JWT（JSON Web Token）的自包含性，紧凑性以及防篡改机制，使得ID Token可以安全的传递给第三方客户端程序并且容易被验证。此外还提供了UserInfo的接口，用户获取用户的更完整的信息。

![img](https://img2020.cnblogs.com/blog/1468246/202005/1468246-20200513161622432-944652564.png)

#### 8、刷新令牌（Refresh Token）

access token 是客户端访问资源服务器的令牌。拥有这个令牌代表着得到用户的授权。然而，这个授权应该是临时的，有一定有效期。这是因为，access token 在使用的过程中可能会泄露。给 access token 限定一个较短的有效期可以降低因 access token 泄露而带来的风险。

refresh token 的作用是用来刷新 access token。鉴权服务器提供一个刷新接口，例如：

```
http://xxx.xxx.com/refresh?refreshtoken=&client_id=
```

