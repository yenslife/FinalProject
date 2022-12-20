using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    public CharacterController characterController;
    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()//��k
    {   //�ĥΪ������ܪ���y�Ъ��覡
        //�@�B�V�k��
        //if (Input.GetKey("d"))//��J.�Ӧ���L(��d��)
        //{
        //    this.gameObject.transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * speed);
        //}  //�����O.�o�Ӫ���.�y�Шt��.�첾(delta�V�q)

        ////�G�B�V�����F�̷Ӥ@�B���@�k�|�o�{�����t�ܧ֡A�]���n���WTime.deltaTime�ө���C
        //if (Input.GetKey("a"))
        //{
        //    this.gameObject.transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * speed);
        //}
        ////�V�W�� //�i�H�����ϥ�Vector���ݩ�Vector2.up�A�N���ݭnnew�@���ܼ�
        //if (Input.GetKey("w"))
        //{
        //    this.gameObject.transform.Translate(new Vector3(0, 0, 1) * Time.deltaTime * speed);
        //}
        ////�V�U��
        //if (Input.GetKey("s"))
        //{
        //    this.gameObject.transform.Translate(new Vector3(0, 0, -1) * Time.deltaTime * speed);
        //}

        // 用角度進行方向控制
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }

    }
}