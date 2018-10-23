using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour 
{
	Rigidbody2D rigid;

	/// <summary>
	/// 何段階ジャンプできるか
	/// </summary>
	[SerializeField]
	uint maxJumpSteps = 1;

	/// <summary>
	/// 現在何段階目のジャンプか
	/// </summary>
	uint jumpCount = 0;

	void Start()
	{
		rigid = GetComponent<Rigidbody2D> ();
	}

	//本来はUpdateの中ではなく、FixedUpdateの中で速度を変更する
	//入力周りはUpdateの中で取得する
	void Update()
	{
		Move ();

		if (Input.GetKeyDown (KeyCode.Space))
			Jump ();
	}

	//テスト用に書いた移動処理なので参考にしないほうが良い
	void Move()
	{
		if (Input.GetKey (KeyCode.RightArrow))
			rigid.AddForce (Vector2.right * 3.0f);
		else if (Input.GetKey (KeyCode.LeftArrow))
			rigid.AddForce (Vector2.left * 3.0f);
		else
			rigid.velocity = new Vector2 (0.0f,rigid.velocity.y);

		if (rigid.velocity.x > 3.0f)
			rigid.velocity = new Vector2 (3.0f, rigid.velocity.y);
	}

	//ジャンプ処理
	void Jump()
	{
		//最大の段階のジャンプを超えてたらジャンプしない
		if (jumpCount >= maxJumpSteps)
			return;

		//ジャンプの力を加える
		rigid.AddForce (Vector2.up * 5.0f, ForceMode2D.Impulse);

		//ジャンプの段階を１段階あげる
		jumpCount++;
	}

	/// <summary>
	/// 自分が何かと接触した時に自動的に呼ばれる関数
	/// </summary>
	/// <param name="other">接触しているオブジェクトの情報</param>
	void OnCollisionEnter2D(Collision2D other)
	{
		//自分があるオブジェクトと接触しているポイントを一つづつ調べる
		foreach (var contact in other.contacts) 
		{
			//自分から接触ポイントへのベクトル
			Vector2 dir = contact.point - (Vector2)transform.position;

			//接触しているゲームオブジェクトの下向きのベクトル
			Vector2 contactObjectUp = -contact.collider.gameObject.transform.up;

			//接触しているオブジェクトの下向きのベクトルと自身から接触しているポイントへのベクトルの
			//角度が１０度未満であった場合にジャンプの段階数のリセットする
			if (Vector2.Angle (contactObjectUp, dir) < 10.0f)
				jumpCount = 0;

			break;
		}
	}
}
