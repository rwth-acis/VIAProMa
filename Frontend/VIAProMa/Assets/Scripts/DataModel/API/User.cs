using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User : IListViewItemData
{
    [SerializeField] private DataSource source;
    [SerializeField] private int id;
    [SerializeField] private string userName;
    [SerializeField] private string firstName;
    [SerializeField] private string lastName;
    [SerializeField] private string profileImageUrl;

    public DataSource Source { get => source;}
    public int Id { get => id; }
    public string UserName { get => userName;  }
    public string FirstName { get => firstName; }
    public string LastName { get => lastName; }
    public string ProfileImageUrl { get => profileImageUrl; }


}
