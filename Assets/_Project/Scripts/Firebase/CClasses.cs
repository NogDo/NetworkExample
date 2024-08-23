using System;

[Serializable]
public class UserData
{
    public enum EClass
    {
        WARRIOR,
        WIZARD,
        ROGUE,
        ARCHER
    }

    public string userId;
    public string userName;
    public int level;
    public EClass characterClass;
    public string address;

    // Json을 역직렬화 하여 객체를 생성하기 위해 필요한 기본생성자
    public UserData() { }

    public UserData(string userId, string userName, int level, EClass characterClass, string address)
    {
        this.userId = userId;
        this.userName = userName;
        this.level = level;
        this.characterClass = characterClass;
        this.address = address;
    }

    public UserData(string userId)
    {
        this.userId = userId;
        userName = "무명의전사";
        level = 1;
        characterClass = EClass.WARRIOR;
        address = "None";
    }
}

[Serializable]
public class Message
{
    public string sender;
    public string message;
    public long sendTime;

    public DateTime GetSendTime()
    {
        return new DateTime(sendTime);
    }
}