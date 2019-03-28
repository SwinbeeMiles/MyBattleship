''' <summary>
''' AttackResult gives the result after a shot has been made.
''' </summary>
Public Class AttackResult
    Private _Value As ResultOfAttack
    Private _Ship As Ship
    Private _Text As String
    Private _Row As Integer
    Private _Column As Integer

    ''' <summary>
    ''' The result of the attack
    ''' </summary>
    ''' <value>The result of the attack</value>
    ''' <returns>The result of the attack</returns>
    Public ReadOnly Property Value() As ResultOfAttack
        Get
            Return _Value
        End Get
    End Property

    ''' <summary>
    ''' The ship, if any, involved in this result
    ''' </summary>
    ''' <value>The ship, if any, involved in this result</value>
    ''' <returns>The ship, if any, involved in this result</returns>
    Public ReadOnly Property Ship() As Ship
        Get
            Return _Ship
        End Get
    End Property

    ''' <summary>
    ''' A textual description of the result.
    ''' </summary>
    ''' <value>A textual description of the result.</value>
    ''' <returns>A textual description of the result.</returns>
    ''' <remarks>A textual description of the result.</remarks>
    Public ReadOnly Property Text() As String
        Get
            Return _Text
        End Get
    End Property

    ''' <summary>
    ''' The row where the attack occurred
    ''' </summary>
    Public ReadOnly Property Row() As Integer
        Get
            Return _Row
        End Get
    End Property

    ''' <summary>
    ''' The column where the attack occurred
    ''' </summary>
    Public ReadOnly Property Column() As Integer
        Get
            Return _Column
        End Get
    End Property

    ''' <summary>
    ''' Set the _Value to the PossibleAttack value
    ''' </summary>
    ''' <param name="value">either hit, miss, destroyed, shotalready</param>
    Public Sub New(ByVal value As ResultOfAttack, ByVal text As String, ByVal row As Integer, ByVal column As Integer)
        _Value = value
        _Text = text
        _Ship = Nothing
        _Row = row
        _Column = column
    End Sub

    ''' <summary>
    ''' Set the _Value to the PossibleAttack value, and the _Ship to the ship
    ''' </summary>
    ''' <param name="value">either hit, miss, destroyed, shotalready</param>
    ''' <param name="ship">the ship information</param>
    Public Sub New(ByVal value As ResultOfAttack, ByVal ship As Ship, ByVal text As String, ByVal row As Integer, ByVal column As Integer)
        Me.New(value, text, row, column)
        _Ship = ship
    End Sub

    ''' <summary>
    ''' Displays the textual information about the attack
    ''' </summary>
    ''' <returns>The textual information about the attack</returns>
    Public Overrides Function ToString() As String
        If _Ship Is Nothing Then
            Return Text
        End If

        Return Text + " " + _Ship.Name
    End Function
End Class
