using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;
using System.Data;

public class CSqlManager : MonoBehaviour
{
    #region static ����
    public static CSqlManager Instance { get; private set; }
    #endregion

    #region public ����
    public string oServerIP;
    #endregion

    #region private ����
    MySqlConnection connection;

    string oDBName = "game";
    string oTableName = "users";
    string oRootPassword = "1234";

    string oCurrentUID;
    #endregion

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        DataBaseConnect();
    }

    /// <summary>
    /// SQL �����ͺ��̽��� �����Ѵ�.
    /// </summary>
    void DataBaseConnect()
    {
        string config = $"server={oServerIP};port=3306;database={oDBName};uid=root;pwd={oRootPassword};charset=utf8";

        connection = new MySqlConnection(config);
        connection.Open();
    }

    /// <summary>
    /// �����ͺ��̽��� �ִ� ������ Ž���� �α����� �õ��Ѵ�.
    /// </summary>
    /// <param name="email">�̸���</param>
    /// <param name="password">��й�ȣ</param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    public void Login(string email, string password, Action<string> successCallback, Action failureCallback)
    {
        string passwordHash = string.Empty;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder st = new StringBuilder();
            foreach (byte b in hashArray)
            {
                st.Append(b.ToString("X2"));
            }

            passwordHash = st.ToString();
        }

        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = $"select * from {oTableName} where email = '{email}' and pw = '{passwordHash}';";

        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        DataSet set = new DataSet();

        adapter.Fill(set);

        bool isLoginSuccess = set.Tables.Count > 0 && set.Tables[0].Rows.Count > 0;

        if (isLoginSuccess)
        {
            DataRow row = set.Tables[0].Rows[0];

            oCurrentUID = row["uid"].ToString();
            successCallback?.Invoke(row["name"].ToString());
        }

        else
        {
            failureCallback?.Invoke();
        }
    }

    /// <summary>
    /// �����ͺ��̽��� ���ο� ȸ�� ������ �߰��Ѵ�.
    /// </summary>
    /// <param name="email">�̸���</param>
    /// <param name="password">��й�ȣ</param>
    /// <param name="nickName">�г���</param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    public void Signup(string email, string password, string nickName, Action successCallback, Action failureCallback)
    {
        string passwordHash = string.Empty;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder st = new StringBuilder();
            foreach (byte b in hashArray)
            {
                st.Append(b.ToString("X2"));
            }

            passwordHash = st.ToString();
        }

        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = $"insert into users(email, pw, name, level, class) values('{email}', '{passwordHash}', '{nickName}', '1', '0');";

        int queryCount = cmd.ExecuteNonQuery();

        if (queryCount > 0)
        {
            successCallback?.Invoke();
        }

        else
        {
            failureCallback.Invoke();
        }
    }

    /// <summary>
    /// �����ͺ��̽��� �ִ� ȸ�� ������ �̸��� �����Ѵ�.
    /// </summary>
    /// <param name="nickName">������ �г���</param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    public void ChangeNickName(string nickName, Action successCallback, Action failureCallback)
    {
        MySqlCommand cmd = new MySqlCommand();
        cmd.Connection = connection;
        cmd.CommandText = $"update {oTableName} set name = '{nickName}' where uid = {oCurrentUID};";

        int queryCount = cmd.ExecuteNonQuery();

        if (queryCount > 0)
        {
            successCallback?.Invoke();
        }

        else
        {
            failureCallback?.Invoke();
        }
    }
}