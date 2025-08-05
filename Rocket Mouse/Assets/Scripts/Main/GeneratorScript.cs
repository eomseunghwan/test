using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    // �߰��� �� �������� �����ϴ� �迭
    public GameObject[] availableRooms;

    // ���� ���ӿ� �߰��Ǿ� �ִ� �� ������Ʈ���� ����� ����Ʈ (�迭�� ����)
    // currentRooms = ���� ���� �� ���� �߰� �Ǹ� ���� �߰��ǰ� ���� �Ǵ°� �ݺ� �Ǳ� ������ �迭�� ���� ���� List �� ��� �Ѵ�
    public List<GameObject> currentRooms;

    // ���� ȭ���� ���� ũ��
    float screenWidthInPoints;

    // �ٴ� ������Ʈ�� �̸��� �����ϴ� ������ �ƴ� ��� (ȭ���� ���� ũ�⸦ �������� ���� �� �ٴ� ������Ʈ�� ȭ�� �� ũ�Ⱑ �Ȱ��ƾ� �� (Ceilin �� ����))
    // const = ��� = ���� �ʱ�ȭ �� �� ���� ������ ����� ����
    const string floor = "Floor";

    // ������ �� ���� �ڵ� ����
    // �߰��� ������Ʈ �������� �����ϴ� �迭
    public GameObject[] availableObjects;

    // ���� ���ӿ� �߰��Ǿ� �ִ� ������Ʈ���� ����� ����Ʈ
    public List<GameObject> objects;

    // ������Ʈ �� �ּ�, �ִ� �Ÿ� (X��)
    public float objectMinDistance; // 5f
    public float objectMaxDistance; // 10f

    // ������Ʈ�� �ּ�, �ִ� Y�� ��ġ
    public float objectMinY;        // -1.4f
    public float objectMaxY;        // 1.4f

    // ������Ʈ�� ȸ�� �� ����
    public float objectMinRotation; //-45f
    public float objectMaxRotation; // 45f

    public FeverController fc;

    private void Start()
    {
        // Camera.main : �±װ� MainCamera �� ī�޶� ������Ʈ�� Camera ������Ʈ ��ü
        // ���� ī�޶��� ũ�⸦ ������ ���� ȭ���� ���α��̸� ����
        float height = 2.0f * Camera.main.orthographicSize;

        // ���α��̿� ȭ���� ������ �̿��ؼ� ���� ȭ���� ���α��̸� ���
        // ȭ�� ������ 1.6 : 1 �� �� aspect �� 1.6 �� ���� �Ǿ� �ִ�
        screenWidthInPoints = height * Camera.main.aspect;
    }

    private void FixedUpdate()
    {
        GeneratorRoomIfRequired();
        GenerateObjectsIfRequired();
    }

    public void RemoveLasers()
    {
        // ������ ������Ʈ�� ����� ������ ����Ʈ
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (var obj in objects)
        {
            if (obj.CompareTag("Laser"))
            {
                objectsToRemove.Add(obj);
            }
        }

        // ������Ʈ ���� ����� ��ü ������Ʈ�� ����
        foreach (var obj in objectsToRemove)
        {
            // ��� (����Ʈ) ���� ����
            objects.Remove(obj);

            // ��� (����Ʈ) ���� ���ŵ� ���� ���� ������Ʈ�� ����
            Destroy(obj);
        }
    }

    private void GenerateObjectsIfRequired()
    {
        // �÷��̾� ������Ʈ�� X�� ��ġ
        float playerX = transform.position.x;

        // ������Ʈ�� ������ ���� ��ġ�� ���
        // removeObjectX = �÷��̾� X�� ��ġ���� ���� ȭ�� ���α��̸� �� ��
        float removeObjectX = playerX - screenWidthInPoints;

        // ������Ʈ�� �߰��� ���� ��ġ�� ���
        // addObjectX = �÷��̾� X�� ��ġ���� ���� ȭ�� ���α��̸� ���� ��
        float addObjectX = playerX + screenWidthInPoints;

        // ���� �ָ��ִ� (�����ʿ� �ִ�) ������Ʈ�� ��ġ
        float farthestObjectX = 0;

        // ������ ������Ʈ�� ����� ������ ����Ʈ
        List<GameObject> objectsToRemove = new List<GameObject>();

        // ���� �����Ǿ� �ִ� ������Ʈ���� ��� �ݺ�
        foreach (var obj in objects)
        {
            // ������Ʈ�� X�� ��ġ�� �����ͼ� objX �� ����
            float objX = obj.transform.position.x;

            // ���� �����ʿ� �ִ� ������Ʈ�� ��ġ�� ����
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            // ������ ������Ʈ�� ������Ʈ ���� ���� ��ġ���� ���ʿ� �ִٸ� ���� ��Ͽ� �߰�
            if (objX < removeObjectX)
            {
                objectsToRemove.Add(obj);
            }
        }

        // ������Ʈ ���� ����� ��ü ������Ʈ�� ����
        foreach (var obj in objectsToRemove)
        {
            // ��� (����Ʈ) ���� ����
            objects.Remove(obj);

            // ��� (����Ʈ) ���� ���ŵ� ���� ���� ������Ʈ�� ����
            Destroy(obj);
        }

        // ���� �����ʿ� �ִ� ������Ʈ�� ������Ʈ ���� ���� ��ġ���� ���ʿ� �ִٸ� ���ο� ������Ʈ�� �߰�
        if (farthestObjectX < addObjectX)
        {
            AddObject(farthestObjectX);
        }
    }

    private void AddObject(float lastObjectX)
    {
        // ������ ������Ʈ�� �迭 �ε����� �������� ����
        int randomIndex = Random.Range(0, availableObjects.Length);

        while (fc.fever)
        {
            if (!availableObjects[randomIndex].CompareTag("Laser"))
            {
                break;
            }

            randomIndex = Random.Range(0, availableObjects.Length);
        }

        // ������ ������Ʈ ����
        GameObject obj = Instantiate(availableObjects[randomIndex]);

        // ���� ������ ������Ʈ�� X�� ��ġ�� �������� ����
        float objectPositionX = lastObjectX + Random.Range(objectMinDistance, objectMaxDistance); // 5 ~ 10

        // ���� ������ ������Ʈ�� Y�� ��ġ�� �������� ����
        float randomY = Random.Range(objectMinY, objectMaxY);

        // ������ ������Ʈ�� ������ ���� ��ġ�� ��ġ��
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        // ���� ������ ������Ʈ�� ȸ������ ���� �� ���������� ����
        float rotation = Random.Range(objectMinRotation, objectMaxRotation);

        // ������ ���� ȸ������ ����
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);

        // ���� �߰��� ������Ʈ ��Ͽ� �߰�
        objects.Add(obj);
    }

    private void AddRoom(float farthestRoomEndX)
    {
        // ���� �� ���� �迭 �ε��� ���� ������ ����
        int randomRoomIndex = Random.Range(0, availableRooms.Length);

        // �� ������Ʈ ����
        GameObject room = Instantiate(availableRooms[randomRoomIndex]);

        // room ������Ʈ�� �ڽ� ������Ʈ �� floor �� ã�� ����ũ�⸦ ������
        // �ٴ� ������Ʈ�� ����ũ�⸦ �̿��Ͽ� ���� ����ũ�⸦ ������
        float roomWidth = room.transform.Find(floor).localScale.x;

        // ���� ������ ���� �߽� ��ġ�� ���
        float roomCenter = farthestRoomEndX + roomWidth / 2;

        // ���� ���� ��ġ�� ���� ��ġ ��Ŵ
        room.transform.position = new Vector3(roomCenter, 0, 0);

        // ���� �߰��� �� ��Ͽ� �߰�
        currentRooms.Add(room);
    }

    private void GeneratorRoomIfRequired()
    {
        // ������ ���� ����� ������ ����Ʈ
        List<GameObject> roomsToRemove = new List<GameObject>();

        // �̹� �����ӿ� �� ������Ʈ�� �������� ����
        bool addRooms = true;

        // �÷��̾� ������Ʈ�� x�� ��ġ ��
        float playerX = transform.position.x;

        // ���� ������ ���� ��ġ�� ��� (�÷��̾� x ��ġ ������ ���� ȭ�� ���� ũ�� ���� �A �� �� ���� ��ġ�� ���� ����)
        float removeRoomX = playerX - screenWidthInPoints;

        // ���� �߰��� ���� ��ġ�� ��� (�÷��̾� x ��ġ ������ ���� ȭ�� ���� ũ�� ���� ���� �� �� ���� ��ġ�� ������ ���� �߰� ���� �� �� ������ ���������� ���� �ϳ��� �� ���� ��� �߰� ���� �� ��)
        float addRoomX = playerX + screenWidthInPoints;

        // ���� �ָ� �ִ� (�����ʿ� �ִ�) ���� ������ �� ��ġ
        float farthestRoomEndX = 0;

        // foreach ���� ���� ���� ���ϸ� �ȵǱ� ������ ������ ���� üũ�� �� �� �� ���� ��Ͽ� �߰� ������ �Ʒ� foreach ���� ����
        // ���� �����Ǿ� �ִ� ����� ��� �ݺ�
        foreach (var room in currentRooms)
        {
            // �� ������Ʈ�� ���� ũ�� (�ٴ� ������Ʈ�� ���� ũ��) �� ������
            float roomWidth = room.transform.Find(floor).localScale.x;

            // �� ������Ʈ (�� ������Ʈ�� ���߾�) �� ���� �� ��ġ�� ������
            // �� ��ġ (�� ������Ʈ�� ���߾�) ���� �� ���α����� ������ ���� ���� ���� ��ġ�� ���
            float roomStartX = room.transform.position.x - roomWidth / 2;

            // �� ���� ��ġ���� �� ���α��̸� ���� ������ ��ġ�� ���
            float roomEndX = roomStartX + roomWidth;

            // ��� ���� ���� ��ġ�� �� ���� ���� ��ġ ���� �����ʿ� �ִٸ� ���� ���� X
            // addRoomX = �÷��̾� ��ġ���� ���� ȭ�� ���α��̸� ���� ��
            // �����ʿ� ���� �ϳ��� ������ if ture �� �Ǽ� ���� �߰� ���� ����
            if (roomStartX > addRoomX)
            {
                addRooms = false;
            }

            // ���� ������ ��ġ�� �� ���� ���غ��� �� ���ʿ� �ִٸ� �� ���� ��Ͽ� �߰�
            if (roomEndX < removeRoomX)
            {
                roomsToRemove.Add(room);
            }

            // ���� �����ʿ� �ִ� ���� ������ ��ġ �� ����
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
        }

        // ���� foreach �� �� �� ���� ���� foreach ���� ���� ��Ͽ� �ִ� ���� ����
        // �� ���� ����� ��ü ���� ����
        foreach ( var room in roomsToRemove)
        {
            // ��� (����Ʈ) ���� ����
            currentRooms.Remove(room);

            // ���� ���� ������Ʈ�� ����
            Destroy(room);
        }

        // �� �߰� ���� Ȯ���Ͽ� �� �߰�
        if (addRooms)
        {
            AddRoom(farthestRoomEndX);
        }
    }
}
