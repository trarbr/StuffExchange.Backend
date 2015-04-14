module Bootstrapper

open Nancy
open Nancy.Authentication.Token
open Nancy.Bootstrapper
open Nancy.TinyIoc

type Bootstrapper() =
    inherit DefaultNancyBootstrapper()

    override this.ConfigureApplicationContainer(container : TinyIoCContainer) = 
        let tokenizer = Tokenizer()
        container.Register<ITokenizer>(tokenizer) |> ignore

    override this.RequestStartup(container : TinyIoCContainer, pipelines : IPipelines, 
                                 context : NancyContext) =
        TokenAuthentication
            .Enable(pipelines, TokenAuthenticationConfiguration(container.Resolve<ITokenizer>()))

//    JUST SAY NO TO HTML
//    override this.ConfigureConventions(NancyConventions nancyConventions) =
//        base.ConfigureConventions(NancyConvensions)
