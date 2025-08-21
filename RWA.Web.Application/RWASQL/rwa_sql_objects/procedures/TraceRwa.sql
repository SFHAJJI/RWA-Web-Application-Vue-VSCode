
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'TraceRwa'))
DROP PROCEDURE TraceRwa
GO


CREATE PROCEDURE TraceRwa 
    @section NVARCHAR(250),
    @step NVARCHAR(250),
    @info NVARCHAR(250) = ''
AS
BEGIN

	DELETE RWA_TRACE where DT<=DATEADD(DAY, -21, getdate());

	INSERT INTO RWA_TRACE (LOGIN, SECTION, STEP, INFO, DT) VALUES (user_name(), @section, @step, @info, getdate())

END
GO

