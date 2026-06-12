package com.example.linkarchive.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;

@Configuration
public class SecurityConfig {
    
    @Bean
    public SecurityFilterChain filterChain(HttpSecurity http) throws Exception {
        http
            .authorizeHttpRequests(auth -> 
                auth.requestMatchers(
                    "/user/signup",
                    "/h2-console/**",
                    "/css/**",
                    "/js/**",
                    "/img/**"
                ).permitAll()
                .anyRequest().authenticated()
            )
            .formLogin(form ->
                form.loginPage("/login")
                    .defaultSuccessUrl("/links", true)
                    .permitAll()
            )
            .logout(logout ->
                logout.logoutSuccessUrl("/login")
            );
            
            //.csrf(csrf -> csrf.disable());

            // h2 허용
            http.csrf(csrf -> 
                csrf.ignoringRequestMatchers("/h2-console/**")
            );

            http.headers(headers -> 
                headers.frameOptions(frame -> 
                    frame.sameOrigin())
            );

            return http.build();
    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }
}
