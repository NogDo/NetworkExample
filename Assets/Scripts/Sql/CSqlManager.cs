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
    #region static 변수
    public static CSqlManager Instance { get; private set; }
    #endregion

    #region public 변수
    public string oServerIP;
    #endregion

    #region private 변수
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
    /// SQL 데이터베이스에 연결한다.
    /// </summary>
    void DataBaseConnect()
    {
        string config = $"server={oServerIP};port=3306;database={oDBName};uid=root;pwd={oRootPassword};charset=utf8";

        connection = new MySqlConnection(config);
        connection.Open();
    }

    /// <summary>
    /// 데이터베이스에 있는 정보를 탐색해 로그인을 시도한다.
    /// </summary>
    /// <param name="email">이메일</param>
    /// <param name="password">비밀번호</param>
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
    /// 데이터베이스에 새로운 회원 정보를 추가한다.
    /// </summary>
    /// <param name="email">이메일</param>
    /// <param name="password">비밀번호</param>
    /// <param name="nickName">닉네임</param>
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
    /// 데이터베이스에 있는 회원 정보의 이름을 변경한다.
    /// </summary>
    /// <param name="nickName">변경할 닉네임</param>
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