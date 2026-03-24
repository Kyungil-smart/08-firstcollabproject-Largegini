
// 작성자 : 한성우
// 상속받는 개체를 자동으로 추리기 위한 인터페이스
// 데이터 테이블은 모두 이 인터페이스를 상속 받아야 함
public interface IDataTableInfo
{
    bool LoadCSVFile(string path);
}