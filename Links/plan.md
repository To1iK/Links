* [ ] Розробити механізм переходу транзакцій
    * Розробка бібліотеки операцій
    * можливість створювати свої операції


В чому проблема? 
   1. Як подавати користувачу можливі варіанти
   2. Як має опрацьовуватись логіка 
   3. Де має зберігатись логіка
   4. В який момент має опрацьовуватись логіка
   5. Чим обмежена логіка щодо управління транзакціями

* [ ] Додати контролер для sql
    * [ ] Отримання параметрів в форматі json
    * [ ] Отримання параметрів в посиланні
* [ ] Перенести контролер для роботи з контентом
* [ ] Перенести механізм формування вузлів

все починається з ініціалізації
є етап ініціатор який стає потоком
на цьому етапі можливі такі варіанти
 користувачу видається форма з реквізитами
 користувач заповнює форму
    користувач відправляє дані на 1 наступний етап
    Користувач відправляє дані на кілька етапів одночасно
    У користувача є вибір між кількома етапами
    

    declare @ui int
set @ui = 1

declare @pn int
set @pn = 0

SELECT n.[id]    
      ,[ParentNodeId]
      ,[NodeName]   
	  ----
	  ,na.*
	  ---
	  ,ug.*
  FROM [Links].[dbo].[Nodes] n
join [Node_Access] na on na.NodeId = n.id --and na.UserId = @ui 
left join UserGroups ug on (ug.GroupId=na.GroupId and ug.UserId = @ui) 
 where na.UserId = @ui or ug.UserId = @ui
  --or na.GroupId=

    select max(na.id) as id
  ,NodeId as NodeId
  ,max(na.userId) as userId
  ,max(na.groupId) as groupId
  ,max(na.accessLevel) as accessLevel
  from [Node_Access] na
  join [Nodes] n on na.NodeId = n.id
  left join UserGroups ug on (ug.GroupId=na.GroupId and ug.UserId = @ui) 
 where na.UserId = @ui or ug.UserId = @ui
 group by 
 NodeId
