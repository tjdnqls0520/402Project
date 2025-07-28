using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsUI : MonoBehaviour
{
    public PlayerMouseMovement player;

    public Slider maxDistanceSlider;
    public Slider jumpHeightSlider;
    public Slider jumpDurationSlider;
    public Slider dashSpeedSlider;
    public Slider maxHoldTimeSlider;

    // 초기값 저장용
    private float defaultMaxDistance;
    private float defaultJumpHeight;
    private float defaultJumpDuration;
    private float defaultDashSpeed;
    private float defaultMaxHoldTime;

    void Start()
    {
        // 초기값 저장
        defaultMaxDistance = player.maxDistance;
        defaultJumpHeight = player.jumpHeight;
        defaultJumpDuration = player.jumpDuration;
        defaultDashSpeed = player.dashSpeed;
        defaultMaxHoldTime = player.maxHoldTime;

        // 슬라이더에 초기값 세팅
        maxDistanceSlider.value = defaultMaxDistance;
        jumpHeightSlider.value = defaultJumpHeight;
        jumpDurationSlider.value = defaultJumpDuration;
        dashSpeedSlider.value = defaultDashSpeed;
        maxHoldTimeSlider.value = defaultMaxHoldTime;

        // 슬라이더 이벤트 연결
        maxDistanceSlider.onValueChanged.AddListener(value => player.maxDistance = value);
        jumpHeightSlider.onValueChanged.AddListener(value => player.jumpHeight = value);
        jumpDurationSlider.onValueChanged.AddListener(value => player.jumpDuration = value);
        dashSpeedSlider.onValueChanged.AddListener(value => player.dashSpeed = value);
        maxHoldTimeSlider.onValueChanged.AddListener(value => player.maxHoldTime = value);
    }

    public void ResetAllSettings()
    {
        // 값 복원
        maxDistanceSlider.value = defaultMaxDistance;
        jumpHeightSlider.value = defaultJumpHeight;
        jumpDurationSlider.value = defaultJumpDuration;
        dashSpeedSlider.value = defaultDashSpeed;
        maxHoldTimeSlider.value = defaultMaxHoldTime;

        // player에도 즉시 반영됨 (슬라이더 리스너 덕분!)
    }
}
