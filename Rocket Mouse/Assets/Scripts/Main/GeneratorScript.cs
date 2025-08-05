using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    // 추가될 방 프리팹을 저장하는 배열
    public GameObject[] availableRooms;

    // 현재 게임에 추가되어 있는 방 오브젝트들이 저장된 리스트 (배열과 유사)
    // currentRooms = 게임 실행 시 방이 추가 되면 값이 추가되고 삭제 되는게 반복 되기 때문에 배열은 쓰기 힘들어서 List 를 사용 한다
    public List<GameObject> currentRooms;

    // 게임 화면의 가로 크기
    float screenWidthInPoints;

    // 바닥 오브젝트의 이름을 저장하는 변수가 아닌 상수 (화면의 가로 크기를 가져오기 위해 단 바닥 오브젝트랑 화면 의 크기가 똑같아야 함 (Ceilin 도 가능))
    // const = 상수 = 값을 초기화 한 뒤 값이 변하지 말라고 쓴다
    const string floor = "Floor";

    // 레이저 및 코인 자동 생성
    // 추가될 오브젝트 프리팹을 저장하는 배열
    public GameObject[] availableObjects;

    // 현재 게임에 추가되어 있는 오브젝트들이 저장된 리스트
    public List<GameObject> objects;

    // 오브젝트 간 최소, 최대 거리 (X축)
    public float objectMinDistance; // 5f
    public float objectMaxDistance; // 10f

    // 오브젝트의 최소, 최대 Y축 위치
    public float objectMinY;        // -1.4f
    public float objectMaxY;        // 1.4f

    // 오브젝트의 회전 값 범위
    public float objectMinRotation; //-45f
    public float objectMaxRotation; // 45f

    public FeverController fc;

    private void Start()
    {
        // Camera.main : 태그가 MainCamera 인 카메라 오브젝트의 Camera 컴포넌트 객체
        // 메인 카메라의 크기를 가져와 게임 화면의 세로길이를 구함
        float height = 2.0f * Camera.main.orthographicSize;

        // 세로길이와 화면의 비율을 이용해서 게임 화면의 가로길이를 계산
        // 화면 비율이 1.6 : 1 일 때 aspect 에 1.6 이 저장 되어 있다
        screenWidthInPoints = height * Camera.main.aspect;
    }

    private void FixedUpdate()
    {
        GeneratorRoomIfRequired();
        GenerateObjectsIfRequired();
    }

    public void RemoveLasers()
    {
        // 삭제할 오브젝트의 목록을 저장할 리스트
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (var obj in objects)
        {
            if (obj.CompareTag("Laser"))
            {
                objectsToRemove.Add(obj);
            }
        }

        // 오브젝트 삭제 목록의 전체 오브젝트를 제거
        foreach (var obj in objectsToRemove)
        {
            // 목록 (리스트) 에서 제거
            objects.Remove(obj);

            // 목록 (리스트) 에서 제거된 실제 게임 오브젝트를 제거
            Destroy(obj);
        }
    }

    private void GenerateObjectsIfRequired()
    {
        // 플레이어 오브젝트의 X축 위치
        float playerX = transform.position.x;

        // 오브젝트를 제거할 기준 위치를 계산
        // removeObjectX = 플레이어 X축 위치에서 게임 화면 가로길이를 뺀 값
        float removeObjectX = playerX - screenWidthInPoints;

        // 오브젝트를 추가할 기준 위치를 계산
        // addObjectX = 플레이어 X축 위치에서 게임 화면 가로길이를 더한 값
        float addObjectX = playerX + screenWidthInPoints;

        // 가장 멀리있는 (오른쪽에 있는) 오브젝트의 위치
        float farthestObjectX = 0;

        // 삭제할 오브젝트의 목록을 저장할 리스트
        List<GameObject> objectsToRemove = new List<GameObject>();

        // 현재 생성되어 있는 오브젝트들을 모두 반복
        foreach (var obj in objects)
        {
            // 오브젝트의 X축 위치를 가져와서 objX 에 저장
            float objX = obj.transform.position.x;

            // 가장 오른쪽에 있는 오브젝트의 위치를 구함
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            // 생성된 오브젝트가 오브젝트 삭제 기준 위치보다 왼쪽에 있다면 삭제 목록에 추가
            if (objX < removeObjectX)
            {
                objectsToRemove.Add(obj);
            }
        }

        // 오브젝트 삭제 목록의 전체 오브젝트를 제거
        foreach (var obj in objectsToRemove)
        {
            // 목록 (리스트) 에서 제거
            objects.Remove(obj);

            // 목록 (리스트) 에서 제거된 실제 게임 오브젝트를 제거
            Destroy(obj);
        }

        // 가장 오른쪽에 있는 오브젝트가 오브젝트 생성 기준 위치보다 왼쪽에 있다면 새로운 오브젝트를 추가
        if (farthestObjectX < addObjectX)
        {
            AddObject(farthestObjectX);
        }
    }

    private void AddObject(float lastObjectX)
    {
        // 생성할 오브젝트의 배열 인덱스를 랜덤으로 구함
        int randomIndex = Random.Range(0, availableObjects.Length);

        while (fc.fever)
        {
            if (!availableObjects[randomIndex].CompareTag("Laser"))
            {
                break;
            }

            randomIndex = Random.Range(0, availableObjects.Length);
        }

        // 랜덤한 오브젝트 생성
        GameObject obj = Instantiate(availableObjects[randomIndex]);

        // 새로 생성할 오브젝트의 X축 위치를 랜덤으로 구함
        float objectPositionX = lastObjectX + Random.Range(objectMinDistance, objectMaxDistance); // 5 ~ 10

        // 새로 생성할 오브젝트의 Y축 위치를 랜덤으로 구함
        float randomY = Random.Range(objectMinY, objectMaxY);

        // 생성된 오브젝트를 위에서 구한 위치로 배치함
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        // 새로 생성한 오브젝트의 회전값을 범위 내 랜덤값으로 구함
        float rotation = Random.Range(objectMinRotation, objectMaxRotation);

        // 위에서 구한 회전값을 적용
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);

        // 현재 추가된 오브젝트 목록에 추가
        objects.Add(obj);
    }

    private void AddRoom(float farthestRoomEndX)
    {
        // 생성 할 방의 배열 인덱스 에서 랜덤을 구함
        int randomRoomIndex = Random.Range(0, availableRooms.Length);

        // 방 오브젝트 생성
        GameObject room = Instantiate(availableRooms[randomRoomIndex]);

        // room 오브젝트의 자식 오브젝트 중 floor 를 찾아 가로크기를 가져옴
        // 바닥 오브젝트의 가로크기를 이용하여 방의 가로크기를 가져옴
        float roomWidth = room.transform.Find(floor).localScale.x;

        // 새로 생성할 방의 중심 위치를 계산
        float roomCenter = farthestRoomEndX + roomWidth / 2;

        // 계산된 방의 위치로 방을 위치 시킴
        room.transform.position = new Vector3(roomCenter, 0, 0);

        // 현재 추가된 방 목록에 추가
        currentRooms.Add(room);
    }

    private void GeneratorRoomIfRequired()
    {
        // 삭제할 방의 목록을 저장할 리스트
        List<GameObject> roomsToRemove = new List<GameObject>();

        // 이번 프레임에 방 오브젝트를 생성할지 여부
        bool addRooms = true;

        // 플레이어 오브젝트의 x축 위치 값
        float playerX = transform.position.x;

        // 방을 제거할 기준 위치를 계산 (플레이어 x 위치 값에서 게임 화면 가로 크기 값을 뺸 뒤 그 값에 위치한 방을 제거)
        float removeRoomX = playerX - screenWidthInPoints;

        // 방을 추가할 기준 위치를 계산 (플레이어 x 위치 값에서 게임 화면 가로 크기 값을 더한 뒤 그 값에 위치한 지점에 방을 추가 생성 단 그 지점에 오른쪽으로 방이 하나라도 더 있을 경우 추가 생성 안 됨)
        float addRoomX = playerX + screenWidthInPoints;

        // 가장 멀리 있는 (오른쪽에 있는) 방의 오른쪽 끝 위치
        float farthestRoomEndX = 0;

        // foreach 도는 동안 값이 변하면 안되기 때문에 삭제할 값을 체크만 한 뒤 방 삭제 목록에 추가 삭제는 아래 foreach 에서 삭제
        // 현재 생성되어 있는 방들을 모두 반복
        foreach (var room in currentRooms)
        {
            // 방 오브젝트의 가로 크기 (바닥 오브젝트의 가로 크기) 를 가져옴
            float roomWidth = room.transform.Find(floor).localScale.x;

            // 방 오브젝트 (방 오브젝트의 정중앙) 의 왼쪽 끝 위치를 가져옴
            // 방 위치 (방 오브젝트의 정중앙) 에서 방 가로길이의 절반을 빼서 방의 왼쪽 위치를 계산
            float roomStartX = room.transform.position.x - roomWidth / 2;

            // 방 왼쪽 위치에서 방 가로길이를 더해 오른쪽 위치를 계산
            float roomEndX = roomStartX + roomWidth;

            // 모든 방의 왼쪽 위치가 방 생성 기준 위치 보다 오른쪽에 있다면 방을 생성 X
            // addRoomX = 플레이어 위치에서 게임 화면 가로길이를 더한 값
            // 오른쪽에 방이 하나라도 있으면 if ture 가 되서 방을 추가 생성 안함
            if (roomStartX > addRoomX)
            {
                addRooms = false;
            }

            // 방의 오른쪽 위치가 방 삭제 기준보다 더 왼쪽에 있다면 방 삭제 목록에 추가
            if (roomEndX < removeRoomX)
            {
                roomsToRemove.Add(room);
            }

            // 가장 오른쪽에 있는 방의 오른쪽 위치 를 구함
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
        }

        // 위에 foreach 를 다 돈 이후 여기 foreach 에서 삭제 목록에 있는 값을 삭제
        // 방 삭제 목록의 전체 방을 제거
        foreach ( var room in roomsToRemove)
        {
            // 목록 (리스트) 에서 제거
            currentRooms.Remove(room);

            // 실제 게임 오브젝트를 제거
            Destroy(room);
        }

        // 방 추가 여부 확인하여 방 추가
        if (addRooms)
        {
            AddRoom(farthestRoomEndX);
        }
    }
}
