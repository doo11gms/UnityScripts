using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class GameObjectContainer : SerializedMonoBehaviour
{
    [OdinSerialize] bool m_IsVisibleInGame = false;
    [OdinSerialize] List<GameObject> m_Contents = new List<GameObject>();
    public List<GameObject> Contents
    {
        get { return m_Contents; }
    }

    [OdinSerialize, BoxGroup("Filtering")] bool m_TagFiltering = false;
    [OdinSerialize, BoxGroup("Filtering"), EnableIf("m_TagFiltering")] List<string> m_DetectableTags = new List<string>();

    Renderer m_Renderer;

    bool IsContentable(GameObject target)
    {
        if (!m_TagFiltering) return true;

        foreach (var tag in m_DetectableTags)
        {
            if (target.tag == tag) return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsContentable(other.gameObject)) return;
        if (m_Contents.Contains(other.gameObject)) return;

        m_Contents.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsContentable(other.gameObject)) return;
        if (!m_Contents.Contains(other.gameObject)) return;

        m_Contents.Remove(other.gameObject);
    }

    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
        m_Renderer.enabled = m_IsVisibleInGame;
    }

    private void Update()
    {
        m_Renderer.enabled = m_IsVisibleInGame;

        if (m_Contents != null)
        {
            /* コライダ内のオブジェクトが外部から削除されていた場合でも削除します。
             * m_Contents.ToList().ForEachをm_Contents.ForEach()と書いてはいけません。
             * ForEachの処理中にコレクションの追加または削除が発生すると、GetEnumeratorメソッドがInvalidOperationExceptionを投げるためです。
             * この例外を回避するために、ToListメソッドによりリストを複製しています。
             * リストを複製する分、パフォーマンスに影響が出ます。*/
            m_Contents.ToList().ForEach(content =>
            {
                if (content == null) m_Contents.Remove(content);
            });
        }
    }
}