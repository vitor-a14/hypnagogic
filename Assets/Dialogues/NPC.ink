INCLUDE Globals.ink

{ knowNPC == false : -> main | ->already_know }

=== main ===
What do you seek in this empty lands? #speaker:Alyzar
    * [I seek anwswers]
    And you shall find. Follow north until you see a blue flame. Don't stop to seek it until a ghostly woman's voice talk in your ears.
    With that, you maybe find the anwswers...
    -> main
    
    * [I'm sick and need aid]
    Don't need to fool me. You're clearlly not sick at all...
    Thy business in this lands are strange... I don't like you're presence in this place.
    I will ask kindly for you to leave.
    Now.
    -> main
    
    * [Actually, who are you?]
    I'm the first that came to this lands. And also, i will be the last.
    I am the beggining and i shall be the end.
    I am just a observer in this chaotic hell.
    -> main
    
    * ->
    Seems like we don't have anything more to say...
    So, this is a good bye.
    ~ knowNPC = true
-> END

=== already_know ===
We already talked about what was needed. 
-> END











