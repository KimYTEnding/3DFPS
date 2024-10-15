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
    InputAction lookAction; // ���콺 �̵�, ��
    InputAction fireAction;
    InputAction reloadAction;

    CharacterController characterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���� ȭ�� �� Ŀ�� ���
        Cursor.visible = false; // Ŀ�� �����

        InputActionAsset inputActions = GetComponent<PlayerInput>().actions; // PlayerInput ������Ʈ���� input actions�� ������

        moveAction = inputActions.FindAction("Move"); // Move�� �ش��ϴ� �Է°� ��������
        lookAction = inputActions.FindAction("Look"); // Look�� �ش�Ǵ� �Է°� ��������
        fireAction = inputActions.FindAction("Fire"); // Fire�� �ش�Ǵ� �Է°� ��������
        reloadAction = inputActions.FindAction("Reload"); // Reload�� �ش�Ǵ� �Է°� ��������

        characterController = GetComponent<CharacterController>(); // Character Controller ������Ʈ ��������

        horizontalAngle = transform.localEulerAngles.y; // ���� ������Ʈ�� �ٶ󺸴� ���� ��������
        verticalAngle = 0; // ���Ͽ��� ���� �ֽ�

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

        Vector2 moveVector = moveAction.ReadValue<Vector2>(); // move �Է� ����
        Vector3 move = new Vector3(moveVector.x, 0, moveVector.y); // move �Է°��� 3D �ΰ��� ������ ��ġ�ϱ� ���� Vector3�� ����

        if (move.magnitude > 1) // Vector�� ũ�⸦ ���� => �հ� �������� ���ÿ� ������ �� 1.1414��ŭ�� ũ�⸦ ������ �� ���� ��
        {
            move.Normalize(); // Vector�� ũ�⸦ 1�� ����������
        }
        move = move * WalkingSpeed * Time.deltaTime; // �̵��ӵ� �����ֱ�
        move = transform.TransformDirection(move); // ���� gameObject�� �������� Vector ȸ��
        characterController.Move(move); // Character Controller�� ���� Character �̵�

        Vector2 look = lookAction.ReadValue<Vector2>(); // look �Է� ����

        /// ���콺 �¿� ��ȭ
        float turnPlayer = look.x * MouseSens; // ���� ȸ�� �ӵ� ����, ���콺 ���� ����
        horizontalAngle += turnPlayer; // ���ΰ����� �÷��̾� ȸ������ ����
        if (horizontalAngle >= 360) horizontalAngle -= 360; // ���� 360 �̻��� �� 360��ŭ ���ҽ�Ŵ
        if (horizontalAngle < 0) horizontalAngle += 360; // ���� 0 ������ �� 360��ŭ ������Ŵ

        Vector3 currentAngle = transform.localEulerAngles; // ������Ʈ�� localEulerAngles���� ������
        currentAngle.y = horizontalAngle; // ������Ʈ�� localEulerAngles���� ��ȭ�� Angle�� y���� ������
        transform.localEulerAngles = currentAngle; // ���� ������Ʈ�� �ٶ󺸴� ����, ��ȭ ���¸� ����

        /// ���콺 ���� ��ȭ
        float turnCam = look.y * MouseSens; // ���� ȸ�� �ӵ� ����, ���콺 ���� ����
        verticalAngle -= turnCam; // ���� ������ ī�޶� ȸ������ ���� <- ��ü�� ������ �� look.y�� ��ȭ�� Ȯ���ϸ� �� �� �ִ�
        verticalAngle = Mathf.Clamp(verticalAngle, -89f, 89f); // verticalAngle�� ��� ������ ũ�ų� ������ �߶󳻴� �Լ�, �� �ִ񰪰� �ּڰ��� ������
        currentAngle = cameraTransform.localEulerAngles; // ī�޶��� localEulerAngles���� ������
        currentAngle.x = verticalAngle; // ī�޶��� localEulerAngles���� ��ȭ�� Angle�� x���� ������
        cameraTransform.localEulerAngles = currentAngle; // ���� ī�޶� �ٶ󺸴� ����, ��ȭ ���¸� ����

        /// �߷� ����
        verticalSpeed -= gravity * Time.deltaTime; // �ӵ� = ���ӵ� x �ð�
        if (verticalSpeed < -terminalSpeed) // -terminalSpeed���� �������� �ӵ��� ���� ���� ������
        {
            verticalSpeed = -terminalSpeed; // �������� �ӵ��� -terminalSpeed�� ����
        }
        Vector3 verticalMove = new Vector3(0, verticalSpeed, 0); // �������� Vector ����
        verticalMove *= Time.deltaTime; // Vector�� deltaTime ����

        CollisionFlags flag = characterController.Move(verticalMove); // ĳ���Ϳ��� �߷¼ӵ� ����

        if (((flag) & (CollisionFlags.Below | CollisionFlags.None)) != 0)
        { // ĳ������ flag�� Below ��Ʈ�� None ��Ʈ�� ������ / �������� ���°� �ƴϸ�
            verticalSpeed = 0; // �ϰ��ӵ��� 0���� �ʱ�ȭ
        }
        // �� ��İ� �ٸ� ������� �ϰ����� Ȯ��
        //Debug.Log(characterController.isGrounded);

        if (!characterController.isGrounded) // ĳ���Ͱ� ���� �پ����� �ʴٸ�
        {
            if (isGrounded) // ������ ĳ���Ͱ� ���� �پ� �־��ٸ�(isGrounded�� true�̸�)
            {
                groundedTimer += Time.deltaTime; // Ÿ�̸� ����
                if (groundedTimer > 0.5f) // ���� �پ����� ���� �ð��� 0.5�� �̻��� ��
                {
                    isGrounded = false; // isGrounded�� false�� ����
                }
            }
        }
        else // ĳ���Ͱ� �پ��ִٸ�
        {
            isGrounded = true; // isGrounded�� true��
            groundedTimer = 0; // timer �ʱ�ȭ
        }

        /// Weapon Fire��� �߰�
        if (fireAction.WasPressedThisFrame()) // �ش� �����ӿ��� FireAction�� Ȱ��ȭ�Ǹ�
        {
            weapons[currentWeaponIndex].FireWeapon(); // �޼ҵ� ����
        }

        /// Weapon Reload��� �߰�
        if (reloadAction.WasPressedThisFrame()) // �ش� �����ӿ��� ReloadAction�� Ȱ��ȭ�Ǹ�
        {
            weapons[currentWeaponIndex].ReloadWeapon(); // �޼ҵ� ����
        }
    }
    
    public void OnChangeWeapon()
    {
        weapons[currentWeaponIndex].gameObject.SetActive(false); // ���� ���� ��Ȱ��ȭ

        currentWeaponIndex++; // ���� ���� �ε��� ����
        if (currentWeaponIndex > weapons.Count-1) // ����Ʈ�� �ε����� �Ѱ��� ��
        {
            currentWeaponIndex = 0; // �ε��� �ʱ�ȭ
        }

        weapons[currentWeaponIndex].gameObject.SetActive(true); // ���� ���� Ȱ��ȭ
    }

    void OnJump() // new InputSystem ����
    {
        if (isGrounded) // ���� �پ��ִٸ�
        {
            verticalSpeed = JumpSpeed; // jumpSpeed ����
            isGrounded = false; // false ����
        }
    }

    public void OnDie()
    {
        GetComponent<Animator>().SetTrigger("Die");
        GameManager.Instance.PlayerDie();
    }
}
