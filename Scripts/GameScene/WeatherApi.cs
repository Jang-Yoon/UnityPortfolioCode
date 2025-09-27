using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using TMPro;

// https://www.data.go.kr/ (공공데이터포털)

public class WeatherApi : MonoBehaviour
{
    // https://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?serviceKey=gh1%2FMzFOA%2FqW1fuhWjUD56R4L7xqVZNRurLYlXdEky0YHLJ3BizZ0gODHNF9oDaZml76BN4B6N5CrveLBwgmQA%3D%3D&pageNo=1&numOfRows=1000&dataType=json&base_date=20250705&base_time=1800&nx=63&ny=110

    // 수정할 필요 없는 변수는 상수화
    private static string mainUrl = "http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getUltraSrtNcst?";
    private static string key = "serviceKey=gh1%2FMzFOA%2FqW1fuhWjUD56R4L7xqVZNRurLYlXdEky0YHLJ3BizZ0gODHNF9oDaZml76BN4B6N5CrveLBwgmQA%3D%3D";
    private static string page = "&pageNo=1";
    private static string raws = "&numOfRows=1000";
    private static string dataType = "&dataType=json";
    // 유연한 프로그래밍을 위한 변수화
    private string baseDate = "&base_date=";
    private string baseTime = "&base_time=";
    private string x = "&nx=60";
    private string y = "&ny=127";
    [SerializeField] private TextMeshProUGUI tmpWeather;
    private void Start()
    {
        WeatherData();
    }

    private void WeatherData()
    {
        string[] dateTime = GetDateTime();
        Debug.Log(dateTime);
        StartCoroutine(GetWeather(dateTime[0], dateTime[1]));

    }

    // 날짜 및 시간 가져오는 기능
    private string[] GetDateTime()
    {
        DateTime currentTime = DateTime.Now;
        //Debug.Log(currentTime); //            월/일/연도 시간:분:초
        //Debug.Log(currentTime.ToString());//    연도-월-일 오후/오전 시간:분:초
        // 20250706
        // 1. " "기준으로 나눈다
        // 2. 특정 문자열을 특정 문자열로 바꾸는 함수

        string date = currentTime.ToString();
        string[] temp = date.Split(' ');

        // string.Replace("a", "b"); a문자열을 b로 치환
        date = temp[0].Replace("-", "");
        //Debug.Log(date); // 연도월일


        // 만약 정시에서 10분 사이라면 1시간 전의 데이터를 보여줘야 한다.
        if (currentTime.Minute <= 10)
        {
            currentTime = currentTime.AddHours(-1);
        }

        string time = currentTime.Hour.ToString();

        if (currentTime.Hour < 10) time = "0" + time;

        time += "30";

        return new string[] { date, time };
    }

    // 유니티에서 웹과 통신할 때는 코루틴을 사용하면 된다.
    private IEnumerator GetWeather(string date, string time)
    {
        string lastUrl = mainUrl + key + page + raws + dataType + baseDate + date + baseTime + time + x + y;

        Debug.Log(lastUrl);

        // 웹에 요청사항 전달
        using (UnityWebRequest request = UnityWebRequest.Get(lastUrl))
        {
            yield return request.SendWebRequest(); // 응답이 올 때까지 기다린다.

            // 성공 했다면
            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text); // 요청에 대한 응답을 다운받아 문자열로 출력

                // 다운로드한 문자열을 다시 제이슨으로 형변환 => key와 value한 쌍인 데이터 타입
                JObject json = JObject.Parse(request.downloadHandler.text);

                // Json의 각각의 값은 Jtoken이다.
                JToken items = json["response"]["body"]["items"]["item"];

                // for문 우리가 반복 횟수를 지정할 수 있다.
                // foreach문은 항상 모든 순회를 한다.
                // foreach(dataType 변수명 in 컨테이너)
                foreach (JToken item in items)
                {
                    // 0 : 없음 / 1 : 비 / 2 : 비,눈 / 3 : 눈 / 4 : 소나기 / 5 : 빗방울 / 6 : 빗방울눈날림 / 7 : 눈날림
                    // 맑음 : 0
                    // 비 : 1, 4, 5
                    // 눈 : 3, 7
                    // 비와눈 : 2, 6
                    if (item["category"].ToString() == "PTY")
                    {
                        #region gamemanager에 정보 전달
                        switch (item["obsrValue"].ToString())
                        {
                            case "0":
                                GameManager.Instance.SetWeather(false, false);
                                tmpWeather.text = "현재 날씨 : 맑음";
                                break;
                            case "1":
                                GameManager.Instance.SetWeather(true, false);
                                tmpWeather.text = "현재 날씨 : 비";
                                break;
                            case "2":
                                GameManager.Instance.SetWeather(true, true);
                                tmpWeather.text = "현재 날씨 : 비와눈";
                                break;
                            case "3":
                                GameManager.Instance.SetWeather(false, true);
                                tmpWeather.text = "현재 날씨 : 눈";
                                break;
                            case "4":
                                GameManager.Instance.SetWeather(true, false);
                                tmpWeather.text = "현재 날씨 : 비";
                                break;
                            case "5":
                                GameManager.Instance.SetWeather(true, false);
                                tmpWeather.text = "현재 날씨 : 비";
                                break;
                            case "6":
                                GameManager.Instance.SetWeather(true, true);
                                tmpWeather.text = "현재 날씨 : 비와눈";
                                break;
                            case "7":
                                GameManager.Instance.SetWeather(false, true);
                                tmpWeather.text = "현재 날씨 : 눈";
                                break;
                        }
                        #endregion
                    }
                }
            }
            else // 실패 했다면
            {
                // 이 로그를 해석해서 코드를 개선
                Debug.LogError(request.error);
            }
        }
    }
}
