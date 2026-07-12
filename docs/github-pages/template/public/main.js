export default {
    defaultTheme: 'auto',
    iconLinks: [
        {
            icon: 'github',
            href: 'https://github.com/gragra33/Blazing.Mvvm',
            title: 'GitHub'
        }
    ],
    configureHljs: (hljs) => {
        // highlight.js has no built-in Razor grammar, so `razor`/`cshtml` blocks
        // fall back to plain text without this. Markup is delegated to xml and
        // @-blocks to csharp; `@code { ... }` relies on the closing brace being
        // at column 0, which holds for all doc snippets.
        hljs.registerLanguage('razor', () => ({
            name: 'Razor',
            aliases: ['cshtml'],
            contains: [
                hljs.COMMENT(/@\*/, /\*@/),
                {
                    className: 'meta',
                    begin: /^@(page|model|inherits|inject|using|namespace|attribute|implements|layout|typeparam|rendermode|preservewhitespace)\b.*$/
                },
                { begin: /@(code|functions)\s*\{/, end: /^\}/, subLanguage: 'csharp' },
                { begin: /@\{/, end: /\}/, subLanguage: 'csharp' },
                { begin: /@\(/, end: /\)/, subLanguage: 'csharp' },
                { className: 'template-variable', begin: /@[\w.]+/ },
                { begin: /<\/?[A-Za-z][^>]*>/, subLanguage: 'xml' }
            ]
        }));
    }
}
