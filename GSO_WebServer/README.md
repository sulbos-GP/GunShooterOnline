<header>
<div align="center"> <h1> 건 슈터 온라인 웹 서버 </h1> </div>
</header>

<body>
  <div align="center"> <h2> 프로젝트 개요 </h2></div>
  

  
  <div align="center"> <h2> 구현 요약 </h2></div>
  
  #### System
  - [게임 서버 사이클](게임-서버-사이클) : 클라이언트가 매칭을 거쳐 게임 서버에 접속되어 입장되고 종료되는 라이프 사이클
  - [매치메이킹](매치메이킹) : ASP.NET 백그라운드 서비스로 Redis Sorted Set을 활용한 매칭큐와 MMR 매치매이킹 시스템 구현 
  - [게임 서버 관리](게임-서버-관리) : 매칭 요청을 받아 ASP.NET으로 Redis에 등록된 컨테이너 상태를 받아 Docker에서 실행되고 있는 게임 방과 연결해주는 매니저
  - [유저 인증 및 관리](유저-인증-및-관리) : 구글 인증을 통해 유저의 토큰을 관리하고 Redis에 임시 저장하여 다음 요청에 활용
  #### Tool
  - [DB 코드 제너레이터](https://github.com/Apeirogon99/SchemaStructor) : MySQL 데이터베이스 스키마를 분석하여 C# 모델 클래스, Enum을 자동으로 생성해주는 코드 제너레이터
  #### Content
  - [티켓](티켓) : 일정 시간이 지나면 클라이언트에서 폴링하여 게임에 입장가능한 티켓을 받을 수 있는지 요청
  - [일일 미션](일일-미션) : 하루(UTC-9)가 지나면 미션(전투, 수집, 플레이)이 랜덤으로 설정되고 인 게임에서 미션을 진행합니다.
  - [레벨 보상](레벨-보상) : 

  <div align="center"> <h1> System </h1> </div>
  <div align="center"> <h2> 게임 서버 사이클 </h2> </div>

  <div align="center"> <h2> 매치메이킹 </h2> </div>

  <div align="center"> <h2> 게임 서버 관리 </h2> </div>

  <div align="center"> <h2> 유저 인증 및 관리 </h2> </div>

  <div align="center"> <h1> Content </h1> </div>
  <div align="center"> <h2> 티켓 </h2> </div>

  <div align="center"> <h2> 일일 미션 </h2> </div>

  <div align="center"> <h2> 레벨 보상 </h2> </div>


</body>
