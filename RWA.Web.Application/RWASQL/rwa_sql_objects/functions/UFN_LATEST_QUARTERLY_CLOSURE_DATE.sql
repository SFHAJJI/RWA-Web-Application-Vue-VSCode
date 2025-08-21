CREATE OR ALTER FUNCTION dbo.UFN_LATEST_QUARTERLY_CLOSURE_PERIOD()
RETURNS @ClosurePeriodTable TABLE (
    ClosurePeriodLastDate date primary key NOT NULL, -- last calendar day of the month
    ClosurePeriodMonthYearChar nvarchar,
    ClosurePeriodMonthInt integer,
    ClosurePeriodYearInt integer
    )
AS
BEGIN

	DECLARE @ClosurePeriodLastDate date,
            @ClosurePeriodMonthInt int,
			@ClosurePeriodYearInt int

    -- Get the latest quarterly closure date
    -- Quarter closure days is the last day of the most recent month among March, June, September, and December
	SET @ClosurePeriodMonthInt = MONTH(GETDATE())
	SET @ClosurePeriodYearInt = YEAR(GETDATE())

	IF @ClosurePeriodMonthInt <= 3 -- go back to previous year if current month is January, February, or March
		BEGIN
			SET @ClosurePeriodMonthInt = 12
			SET @ClosurePeriodYearInt = @ClosurePeriodYearInt - 1
		END
	ELSE IF @ClosurePeriodMonthInt <= 6  SET @ClosurePeriodMonthInt = 3
	ELSE IF @ClosurePeriodMonthInt <= 9  SET @ClosurePeriodMonthInt = 6
	ELSE IF @ClosurePeriodMonthInt <= 12 SET @ClosurePeriodMonthInt = 9
  
    -- Convert to date
    SET @ClosurePeriodLastDate = DATEADD(month, ((@ClosurePeriodYearInt - 1900) * 12) + @ClosurePeriodMonthInt, -1)

    -- Create return table
    INSERT @ClosurePeriodTable
    SELECT @ClosurePeriodLastDate, CAST(@ClosurePeriodMonthInt as nvarchar) + CAST(@ClosurePeriodYearInt as nvarchar), @ClosurePeriodMonthInt, @ClosurePeriodYearInt

    RETURN
	-- SELECT * FROM dbo.UFN_LATEST_QUARTERLY_CLOSURE_DATE();

END;
GO