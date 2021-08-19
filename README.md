## CF-SRM

使用.NET5 + Angular 11 + NG-ZORRO開發，實現了很多基本功能，方便二次開發。

### 功能

實現了系统管理（用户，角色，菜單），組織管理（部門，職位），審批工作流，内容管理（文章，文件，字典等），代碼生成，日志工具等。

### 演示

地址：http://10.88.1.28/account/login

账号：admin1~admin9

密码：同帳號

### 開發環境

vs + vs code

### 本地運行

Convience.Web\Managent是web端

Convience.Backend是api端

### 本地運行（docker）

cd到src目錄执行docker-compose up -d --build

然後訪問localhost:8888

### 創建項目模板（後端）

cd到src\Convience.Backend目录，执行[dotnet new -i .]，这样就创建了一个convience名称的模板（名称可以在template.json中修改）。然后通过[dotnet new convience -n 项目名]可以创建新项目，新项目的命名空间会被修改为刚才指定的项目名。

### 重要變更

2021/03/13  api從net core3.1 升级到 .net5