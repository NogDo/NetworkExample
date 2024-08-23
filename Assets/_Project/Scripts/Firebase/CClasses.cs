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

    // Json�� ������ȭ �Ͽ� ��ü�� �����ϱ� ���� �ʿ��� �⺻������
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
        userName = "����������";
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