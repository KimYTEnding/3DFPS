using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, Health.IHealthListener
{
    public float WalkingSpeed = 7;
    public float MouseSens = 1;
    public float JumpSpeed = 5;
    public Transform cameraTransform;

    public List<Weapon> weapons;
    int currentWeaponIndex;

    public float gravity = 10;
    public float terminalSpeed = 20;

    float horizontalAngle;
    float verticalAngle;
    public float verticalSpeed = 0;
    bool isGrounded;
    float groundedTimer;

    InputAction moveAction;
    InputAction lookAction; // 마우스 이동, 줌
    InputAction fireAction;
    InputAction reloadAction;

    CharacterController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 게임 화면 내 커서 잠금
        Cursor.visible = false; // 커서 숨기기

        InputActionAsset inputActions = GetComponent<PlayerInput>().actions; // PlayerInput 컴포넌트에서 input actions을 가져옴

        moveAction = inputActions.FindAction("Move"); // Move에 해당하는 입력값 가져오기
        lookAction = inputActions.FindAction("Look"); // Look에 해당되는 입력값 가져오기
        fireAction = inputActions.FindAction("Fire"); // Fire에 해당되는 입력값 가져오기
        reloadAction = inputActions.FindAction("Reload"); // Reload에 해당되는 입력값 가져오기

        characterController = GetComponent<CharacterController>(); // Character Controller 컴포넌트 가져오기

        horizontalAngle = transform.localEulerAngles.y; // 현재 오브젝트가 바라보는 방향 가져오기
        verticalAngle = 0; // 상하에서 정면 주시

        verticalSpeed = 0;
        isGrounded = true;
        groundedTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        Vector2 moveVector = moveAction.ReadValue<Vector2>(); // move 입력 감지
        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y); // move 입력값을 3D 인게임 값으로 대치하기 위해 Vector3로 변경

        if (move.magnitude > 1) // Vector의 크기를 감지 => 앞과 오른쪽을 동시에 눌렀을 때 1.1414만큼의 크기를 가지게 될 때가 예
        {
            move.Normalize(); // Vector의 크기를 1로 고정시켜줌
        }
        move = move * WalkingSpeed * Time.deltaTime; // 이동속도 곱해주기
        move = transform.TransformDirection(move); // 현재 gameObject의 방향으로 Vector 회전
        characterController.Move(move); // Character Controller를 통해 Character 이동

        Vector2 look = lookAction.ReadValue<Vector2>(); // look 입력 감지

        /// 마우스 좌우 변화
        float turnPlayer = look.x * MouseSens; // 가로 회전 속도 변수, 마우스 감도 적용
        horizontalAngle += turnPlayer; // 가로각도에 플레이어 회전값을 더함
        if (horizontalAngle >= 360) horizontalAngle -= 360; // 값이 360 이상일 때 360만큼 감소시킴
        if (horizontalAngle < 0) horizontalAngle += 360; // 값이 0 이하일 때 360만큼 증가시킴

        Vector3 currentAngle = transform.localEulerAngles; // 오브젝트의 localEulerAngles값을 가져옴
        currentAngle.y = horizontalAngle; // 오브젝트의 localEulerAngles값에 변화한 Angle의 y값을 대입함
        transform.localEulerAngles = currentAngle; // 현재 오브젝트가 바라보는 방향, 변화 상태를 적용

        /// 마우스 상하 변화
        float turnCam = look.y * MouseSens; // 상하 회전 속도 변수, 마우스 감도 적용
        verticalAngle -= turnCam; // 세로 각도에 카메라 회전값을 빼줌 <- 물체를 돌려볼 때 look.y값 변화를 확인하면 알 수 있다
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f); // verticalAngle이 어떠한 값보다 크거나 작으면 잘라내는 함수, 즉 최댓값과 최솟값을 정해줌
        currentAngle = cameraTransform.localEulerAngles; // 카메라의 localEulerAngles값을 가져옴
        currentAngle.x = verticalAngle; // 카메라의 localEulerAngles값에 변화한 Angle의 x값을 대입함
        cameraTransform.localEulerAngles = currentAngle; // 현재 카메라가 바라보는 방향, 변화 상태를 적용

        /// 중력 적용
        verticalSpeed -= gravity * Time.deltaTime; // 속도 = 가속도 x 시간
        if (verticalSpeed < -terminalSpeed) // -terminalSpeed보다 떨어지는 속도가 낮은 값을 가지면
        {
            verticalSpeed = -terminalSpeed; // 떨어지는 속도를 -terminalSpeed로 고정
        }
        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0); // 떨어지는 Vector 생성
        verticalMove *= Time.deltaTime; // Vector에 deltaTime 적용

        CollisionFlags flag = characterController.Move(verticalMove); // 캐릭터에게 중력속도 적용

        if (((flag) & (CollisionFlags.Below | CollisionFlags.None)) != 0)
        { // 캐릭터의 flag에 Below 비트나 None 비트가 없으면 / 떨어지는 상태가 아니면
            verticalSpeed = 0; // 하강속도를 0으로 초기화
        }
        // 위 방식과 다른 방법으로 하강상태 확인
        //Debug.Log(characterController.isGrounded);

        if (!characterController.isGrounded) // 캐릭터가 땅에 붙어있지 않다면
        {
            if (isGrounded) // 이전에 캐릭터가 땅에 붙어 있었다면(isGrounded가 true이면)
            {
                groundedTimer += Time.deltaTime; // 타이머 시작
                if (groundedTimer > 0.5f) // 땅에 붙어있지 않은 시간이 0.5초 이상일 때
                {
                    isGrounded = false; // isGrounded를 false로 설정
                }
            }
        }
        else // 캐릭터가 붙어있다면
        {
            isGrounded = true; // isGrounded를 true로
            groundedTimer = 0; // timer 초기화
        }

        /// Weapon Fire기능 추가
        if (fireAction.WasPressedThisFrame()) // 해당 프레임에서 FireAction이 활성화되면
        {
            weapons[currentWeaponIndex].FireWeapon(); // 메소드 실행
        }

        /// Weapon Reload기능 추가
        if (reloadAction.WasPressedThisFrame()) // 해당 프레임에서 ReloadAction이 활성화되면
        {
            weapons[currentWeaponIndex].ReloadWeapon(); // 메소드 실행
        }
    }
    
    public void OnChangeWeapon()
    {
        weapons[currentWeaponIndex].gameObject.SetActive(false); // 현재 무기 비활성화

        currentWeaponIndex++; // 선택 무기 인덱스 증가
        if (currentWeaponIndex > weapons.Count-1) // 리스트의 인덱스를 넘겼을 때
        {
            currentWeaponIndex = 0; // 인덱스 초기화
        }

        weapons[currentWeaponIndex].gameObject.SetActive(true); // 다음 무기 활성화
    }

    void OnJump() // new InputSystem 적용
    {
        if (isGrounded) // 땅에 붙어있다면
        {
            verticalSpeed = JumpSpeed; // jumpSpeed 적용
            isGrounded = false; // false 설정
        }
    }

    public void OnDie()
    {
        GetComponent<Animator>().SetTrigger("Die");
        GameManager.Instance.PlayerDie();
    }
}
