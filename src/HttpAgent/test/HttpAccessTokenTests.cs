// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace HttpAgent.Tests;

public class HttpAccessTokenTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new HttpAccessToken(null!, DateTimeOffset.Now));

    [Fact]
    public void New_ReturnOK()
    {
        var now = DateTimeOffset.Now;
        var accessToken = new HttpAccessToken("token value", now);

        Assert.Equal("token value", accessToken.Value);
        Assert.Equal(now, accessToken.ExpiresAt);
        Assert.Null(accessToken.Scheme);
        Assert.NotNull(accessToken.Items);
        Assert.Empty(accessToken.Items);
        Assert.Null(accessToken.RefreshToken);

        accessToken.RefreshToken = "refresh token value";
        Assert.Equal("refresh token value", accessToken.RefreshToken);
        Assert.Single(accessToken.Items);
        Assert.Equal("refresh token value", accessToken.Items["refresh_token"]);

        Assert.Equal("refresh_token", HttpAccessToken.RefreshTokenKey);

        var accessToken2 = new HttpAccessToken("token value", 1784622666);
        Assert.Equal("token value", accessToken2.Value);
        Assert.Equal("2026/7/21 8:31:06 +00:00", accessToken2.ExpiresAt.ToString());

        const string token =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOjEsIkFjY291bnQiOiJhZG1pbiIsImlhdCI6MTc4NDYyMTQ2NiwibmJmIjoxNzg0NjIxNDY2LCJleHAiOjE3ODQ2MjI2NjYsImlzcyI6ImRvdG5ldGNoaW5hIiwiYXVkIjoicG93ZXJieSBGdXJpb24ifQ.ehnfc9mTeg9HclVWFarDd3T9vOocg2OnQwx7jaMkpqQ";

        var accessToken3 = new HttpAccessToken(token);
        Assert.Equal(token, accessToken3.Value);
        Assert.Equal("2026/7/21 8:31:06 +00:00", accessToken3.ExpiresAt.ToString());
    }

    [Fact]
    public void IsExpired_ReturnOK()
    {
        var date = DateTimeOffset.Parse("2026-01-01 00:00:00");
        var accessToken = new HttpAccessToken("token value", date);
        Assert.True(accessToken.IsExpired());

        var date1 = DateTimeOffset.Now.AddMinutes(10);
        var accessToken2 = new HttpAccessToken("token value", date1);
        Assert.False(accessToken2.IsExpired());
    }

    [Fact]
    public void SetExpiresAt_ReturnOK()
    {
        var now = DateTimeOffset.Now;
        var accessToken = new HttpAccessToken("token value", now);
        accessToken.SetExpiresAt(DateTimeOffset.Parse("2026-01-01 00:00:00 +00:00"));
        Assert.Equal("2026/1/1 0:00:00 +00:00", accessToken.ExpiresAt.ToString());

        accessToken.SetExpiresAt(1784622666);
        Assert.Equal("2026/7/21 8:31:06 +00:00", accessToken.ExpiresAt.ToString());
    }
}