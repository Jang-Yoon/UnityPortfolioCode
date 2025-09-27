using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///  설정 및 관리, 모든 씬에서 사용할 수 있는 공통 데이터를 관리
/// </summary>

// keyword : static
// static -> 이 프로젝트에서 단 하나만 유지하도록 한다. , 메모리에 등록된다.
public static class Config
{
    public static string playerNickName;

    public static bool isAttacker;

    public static bool isRelease = false; // 출시, 테스트 버전 관리
}

public enum SceneList
{
    IntroScene,
    MainScene,
    GameScene
}

