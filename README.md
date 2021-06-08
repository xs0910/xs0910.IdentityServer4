## **环境**

ASP.NET Core 3.1 MVC

IdentityServer4(4.1.2)

## 安装Nuget包

```C#
Install-Package IdentityServer4

Install-Package IdentityServer4.AspNetIdentity

Install-Package IdentityServer4.EntityFramework
    
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore

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

## Quick UI

https://github.com/IdentityServer/IdentityServer4.Quickstart.UI

执行命令：

iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/main/getmain.ps1'))