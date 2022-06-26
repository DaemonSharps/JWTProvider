using Infrastructure.Common;

namespace Common;

public class StringHasherTests
{
    /// <summary>
    /// my default hash site is 32. https://stackoverflow.com/questions/27817282/sha256-giving-44-length-output-instead-64-length
    /// </summary>
    private const int DefaultHashLength = 44;

    [Theory]
    [InlineData("test_salt", "SdcF09HwQpUDSw6B0g0V/bWQ+Xct1pKVn665T8lovxs=")]
    [InlineData("12312312354125124123", "yQi5p1ZSSOsiXjgY1sbJ1OU76/Efy+9yVKYchaU1514=")]
    [InlineData("00000000000000000000000000000", "V1cx573XALQnFjIuYamez+T0gSmJosM0qD0PkKXu5sg=")]
    [InlineData("f", "DJU9dCjoc3btapi5u342MI37EQoRv6gjlBd8suipTWw=")]
    [InlineData("++++++====__", "OG8xN8lxhPhkjgBKjA3sda/9y8gAZwlY37pbDWkR05A=")]
    public void HashWithSaltOnly(string salt, string expected)
    {
        //Arrange
        const string StringToHash = "I_am_test_string00099((())){{}}&&*&^^!@%$!@#$*#%(!#&%)(!^*#@%#@!#CMXMAIFUBJanskfjb,ad/././.";
        var hasher = new StringHasher(StringToHash);
        //Act
        var result = hasher.Hash(salt);
        //Assert
        Assert.NotNull(result);
        Assert.Equal(DefaultHashLength, result.Length);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test_salt", "test_pepper", "eqx8uGbY6Tsk2o4dR6fO07btZnn14E6Rl8Hc3EJOICU=")]
    [InlineData("12312312354125124123", "019725019250915619247019265019256", "R7KmHIAUIslcKFE+oh20hx0ZVw9ET/6tiuhs1voiG6I=")]
    [InlineData("00000000000000000000000000000", "00000000000000000", "Kj5ACuM1jwkaIksKv9suOGoAP2DAzeYMezBpfmPwVTI=")]
    [InlineData("f", "ff1131", "gkLfJt7iJWvqQG1HyPHA6OfRdiuvCd7CPJWynWivu8g=")]
    [InlineData("++++++====__", "+_+))*+_&)_+)", "9gGkaO3tDRWDvCI4rDJFfd4W/vyObB7E0ql++0/DwjI=")]
    public void HashWithSaltAndPepper(string salt, string pepper, string expected)
    {
        //Arrange
        const string StringToHash = "I_am_test_string00099((())){{}}&&*&^^!@%$!@#$*#%(!#&%)(!^*#@%#@!#CMXMAIFUBJanskfjb,ad/././.";
        var hasher = new StringHasher(StringToHash);
        //Act
        var result = hasher.Hash(salt, pepper);
        //Assert
        Assert.NotNull(result);
        Assert.Equal(DefaultHashLength, result.Length);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 1, "Bg==")]
    [InlineData(10, 10, "BwX4v3jggUvhxw==")]
    [InlineData(1000, 40, "Jc6yGVJxS2dcsdB4bkpko1eZzgzxBEM1b1gi3S2j3NDjYHQNcYcFww==")]
    [InlineData(10000, 100, "l8sbyWZ90EkbH0xAItFVRaUYN5YSpFPgOdGQtUU0ipicAYCP8uK2d03LaY/m1zWYQby7uYvUsjcR8pEB3Eo1AxqtJ2PAfpRFRXgK7hx/TZFPsJxM1t2lZu76XaMkpYazyM6rzw==")]
    public void HashWithIterationsAndHashSize(int iterations, int hashSize, string expected)
    {
        //Arrange
        const string StringToHash = "I_am_test_string00099((())){{}}&&*&^^!@%$!@#$*#%(!#&%)(!^*#@%#@!#CMXMAIFUBJanskfjb,ad/././.";
        const string Salt = "I_am_test_Salt_91023120-=-909=-98:'     mmmzzz";
        const string Pepper = "I_am_test_pepper_**&&%$@!@$%^&*()\":<MHTRDSXH";
        var hasher = new StringHasher(StringToHash);
        //Act
        var result = hasher.Hash(Salt, iterations, hashSize, Pepper);
        //Assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NullSaltThrowError()
    {
        //Arrange
        const string StringToHash = "any";
        const string Pepper = "any";
        var hasher = new StringHasher(StringToHash);
        //Act
        var hashAction = () => hasher.Hash(salt: null, Pepper);
        //Assert
        Assert.Throws<ArgumentNullException>(hashAction);
    }

    [Fact]
    public void NullStringToHashThrowError()
    {
        //Arrange
        const string StringToHash = null;
        //Act
        var hashCreation = () => new StringHasher(StringToHash);
        //Assert
        Assert.Throws<ArgumentNullException>(hashCreation);
    }
}
