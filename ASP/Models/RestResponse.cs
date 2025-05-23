﻿namespace ASP.Models;

public class RestResponse // REST uniform interface
{
    public string Service { get; set; } = null!;  // Resource identification
    public RestStatus Status { get; set; } = new();
    public long CacheTime { get; set; } = 0L;
    public string DataType { get; set; } = "empty";
    public Dictionary<string, string> Meta { get; set; } = new();
    public Manipulations Manipulations { get; set; } = new();
    public Object? Data { get; set; }
}

public class RestStatus
{
    public bool IsOk { get; set; } = true;
    public int Code { get; set; } = 200;
    public string Phrase { get; set; } = "Ok";
}

public class Manipulations
{
    public string? Create { get; set; }
    public string? Read { get; set; }
    public string? Update { get; set; }
    public string? Delete { get; set; }
}

/*
 * Proxy:
 *[Browser/Mobile]      [Webhost]           [Firm DB]
 * Frontend      GET     Proxy     GET        Backend
 * shop.firm.ua <---->  firm.ua <------->   123.75.78.94
 *               500               404
 *       could not complete      not found
 *        200 {status: 404}     200 {status: 404}
*/